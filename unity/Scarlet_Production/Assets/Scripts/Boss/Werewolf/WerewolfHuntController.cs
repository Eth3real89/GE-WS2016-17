using System;
using System.Collections;
using UnityEngine;

public class WerewolfHuntController : WerewolfController, AttackCombo.ComboCallback, LungeAttack.LungeAttackCallbacks {

    public AttackCombo m_JumpCombo;
    public LungeAttack m_LungeAttack;

    public Transform m_HuntCenter;
    public float m_HuntDistance;

    public int m_NumParryRequired = 4;
    public PlayerControls m_PlayerControls;
    public PlayerAttackCommand m_PlayerAttackCommand;
    public PlayerDashCommand m_PlayerDashCommand;
    private float m_AttackDmgAmountBefore;
    private float m_RiposteDmgAmountBefore;
    private float m_FinalDmgAmountBefore;

    public PlayerParryCommand m_PlayerParryCommand;
    private float m_BlockTimeBefore;

    public float m_MinHuntTime = 5f;
    public float m_MaxHuntTime = 9f;
    protected float m_StalkSpeed = 0.8f;
    private bool m_JumpSoon;

    public CharacterHealth m_BossHealth;

    public BossTurnCommand m_BossTurn;
    public BossMoveCommand m_BossMove;

    public TutorialPromptController m_Tutorial;

    public float m_SlowMoAmount;
    public float m_SlowMoTime;

    public Animator m_Animator;

    private bool m_Active;
    private BossfightCallbacks m_Callbacks;

    private IEnumerator m_HuntTimer;
    private IEnumerator m_StateEnumerator;
    private IEnumerator m_SlowMoTimer;

    /// <summary>
    /// as opposed to "jumping": hunting = running around
    /// </summary>
    private bool m_Hunting;

    protected bool m_DirectionCounterClockwise;

    public bool m_SuccessfullyRiposted;

    public void StartHuntPhase(BossfightCallbacks callbacks)
    {
        m_NotDeactivated = true;

        m_BossHittable.RegisterInterject(this);
        m_SuccessfullyRiposted = false;

        m_Callbacks = callbacks;
        m_Active = true;
        m_JumpSoon = false;
        m_LungeAttack.m_LungeAttackCallbacks = this;

        m_RiposteDmgAmountBefore = m_PlayerAttackCommand.m_RiposteDamage;
        m_AttackDmgAmountBefore = m_PlayerAttackCommand.m_RegularHitDamage;
        m_FinalDmgAmountBefore = m_PlayerAttackCommand.m_FinalHitDamage;

        m_PlayerAttackCommand.m_RegularHitDamage = 0;
        m_PlayerAttackCommand.m_FinalHitDamage = 0;
        m_PlayerAttackCommand.m_RiposteDamage = m_BossHealth.m_MaxHealth / m_NumParryRequired;
        m_ActiveCombo = null;
        m_ComboActive = false;

        m_BlockTimeBefore = m_PlayerParryCommand.m_OkParryTime;
        m_PlayerParryCommand.m_OkParryTime = 0;
        m_PlayerParryCommand.m_PerfectParryTime = m_PlayerParryCommand.m_PerfectParryTime + m_BlockTimeBefore;

        EventManager.StartListening("user_parry", OnUserFirstParry);
        RegisterEventsForSound();

        LaunchSingleHuntIteration();
    }

    private void Update()
    {
        if (m_Active)
        {
            if (m_BossHealth.m_CurrentHealth <= 0)
            {
                m_PlayerControls.EnableAndUnlock(m_PlayerDashCommand);
                m_PlayerControls.EnableAllCommands();
                EndPhase();
            }
        }
    }

    private IEnumerator EndHuntAfter(float time)
    {
        yield return new WaitForSeconds(time);
        m_JumpSoon = true;

    }

    private bool CheckJump()
    {
        float turnAngle = this.transform.rotation.eulerAngles.y;
        return (Mathf.Abs(((int) turnAngle) % 90) <= 4);
    }

    private void LungeOut()
    {
        m_JumpSoon = false;
        m_Hunting = false;
        m_JumpCombo.m_Callback = this;

        m_BossMove.StopMoving();
        m_JumpCombo.LaunchCombo();
    }

    private void LaunchSingleHuntIteration()
    {
        m_Hunting = true;
        m_JumpSoon = false;

        m_StateEnumerator = ReachHuntDistance();
        StartCoroutine(m_StateEnumerator);
    }

    protected IEnumerator ReachHuntDistance()
    {
        float distanceToCenter = Vector3.Distance(transform.position, m_HuntCenter.transform.position);
        bool initiallyTooClose = distanceToCenter < m_HuntDistance;

        bool wasCloserThanScarlet = distanceToCenter <= Vector3.Distance(m_Scarlet.transform.position, m_HuntCenter.transform.position);

        if (wasCloserThanScarlet)
        {
            float scarletAngle = BossTurnCommand.CalculateAngleTowards(m_Scarlet.transform.position, m_HuntCenter.position);
            m_BossTurn.TurnBossBy(-transform.rotation.eulerAngles.y + scarletAngle + 45);
        }

        while (true)
        {
            float turnAngle;
            distanceToCenter = Vector3.Distance(transform.position, m_HuntCenter.transform.position);

            if (distanceToCenter < m_HuntDistance)
            {
                if (!initiallyTooClose)
                    break;
                turnAngle = BossTurnCommand.CalculateAngleTowards(this.transform, m_HuntCenter);
                turnAngle += 180;
            }
            else
            {
                if (initiallyTooClose)
                    break;
                turnAngle = BossTurnCommand.CalculateAngleTowards(this.transform, m_HuntCenter);
            }

            if (!wasCloserThanScarlet)
            {
                m_BossTurn.TurnBossBy(turnAngle);
            }
            m_BossMove.DoMove(transform.forward.x, transform.forward.z);

            yield return null;
        }

        m_BossMove.StopMoving();

        m_StateEnumerator = DetermineStalkDirection();
        StartCoroutine(m_StateEnumerator);
    }

    protected IEnumerator DetermineStalkDirection()
    {
        m_DirectionCounterClockwise = UnityEngine.Random.value >= 0.5;
        m_DirectionCounterClockwise = false;
        m_Animator.SetTrigger("CrouchTrigger");

        yield return new WaitForSeconds(0.5f);
        if (m_DirectionCounterClockwise)
        {
            m_Animator.SetTrigger("CrouchToStalkL");
        }
        else
        {
            m_Animator.SetTrigger("CrouchToStalkR");
        }

        float t = 0;
        while((t += Time.deltaTime) < 0.6f)
        {
            float angle = CalculateAngleTowardsCenter() + (m_DirectionCounterClockwise ? 90 : -90);
            if (angle < -180)
                angle += 360;
            else if (angle > 180)
                angle -= 360;

            m_BossTurn.TurnBossBy(angle * t / 0.6f);
            yield return null;
        }

        m_StateEnumerator = Stalk();
        StartCoroutine(m_StateEnumerator);
    }

    private float CalculateAngleTowardsCenter()
    {
        float angle = BossTurnCommand.CalculateAngleTowards(this.transform, m_HuntCenter);

        while (angle > 180)
            angle -= 360;
        while (angle < -180)
            angle += 360;
        return angle;
    }

    protected IEnumerator Stalk()
    {
        float time = m_MinHuntTime + UnityEngine.Random.value * (m_MaxHuntTime - m_MinHuntTime);
        m_HuntTimer = EndHuntAfter(time);
        StartCoroutine(m_HuntTimer);

        float angle;
        while (!m_JumpSoon)
        {
            angle = CalculateAngleTowardsCenter() + (m_DirectionCounterClockwise ? 90 : -90);
            m_BossTurn.TurnBossBy(angle);

            Vector3 newPos = transform.position + m_StalkSpeed * transform.forward * Time.deltaTime;
            newPos = m_HuntCenter.transform.position + m_HuntDistance * (newPos - m_HuntCenter.transform.position).normalized;

            transform.position = newPos;
            yield return null;
        }

        float t = 0;
        while((t += Time.deltaTime) < 0.1f)
        {
            angle = CalculateAngleTowardsCenter();
            m_BossTurn.TurnBossBy(angle * t / 0.1f);
            yield return null;
        }
        angle = CalculateAngleTowardsCenter();
        m_BossTurn.TurnBossBy(angle);

        LungeOut();
    }

    private void RunInCircle(float distanceToCenter)
    {
        float angle = BossTurnCommand.CalculateAngleTowards(this.transform, m_HuntCenter) + 90;

        while (angle > 180)
            angle -= 360;
        while (angle < -180)
            angle += 360;

        m_BossTurn.TurnBossBy(angle);
        m_BossMove.DoMove(transform.forward.x, transform.forward.z);
    }

    public new void OnInterruptCombo(AttackCombo combo)
    {
        if (combo == m_JumpCombo)
        {
            if (m_SlowMoTimer != null)
            {
                MLog.Log(LogType.BattleLog, 0, "Hunt Controller: Cancelling Jump Slow Mo");
                StopCoroutine(m_SlowMoTimer);
            }
        }

        if (m_Hunting)
            return;

        if (m_HuntTimer == null)
            StopCoroutine(m_HuntTimer);

        m_HuntTimer = WaitToStandUp();
        StartCoroutine(m_HuntTimer);
    }

    public new void OnComboStart(AttackCombo combo)
    {
        m_PlayerControls.EnableAndUnlock(m_PlayerDashCommand);
        m_PlayerControls.EnableAllCommands();
    }

    public new void OnComboEnd(AttackCombo combo)
    {
        m_PlayerControls.EnableAndUnlock(m_PlayerDashCommand);
        m_PlayerControls.EnableAllCommands();
        StartCoroutine(WaitToStandUp());

//        LaunchSingleHuntIteration();
    }

    private IEnumerator WaitToStandUp()
    {
        yield return new WaitForSeconds(0.5f);
        LaunchSingleHuntIteration();
    }

    public override bool OnHit(Damage dmg)
    {
        m_PlayerControls.EnableAndUnlock(m_PlayerDashCommand);
        m_PlayerControls.EnableAllCommands();

        if (dmg.m_Type == Damage.DamageType.Riposte)
        {
            dmg.OnSuccessfulHit();

            if (!m_SuccessfullyRiposted)
            {
                if (m_SlowMoTimer != null)
                {
                    StopCoroutine(m_SlowMoTimer);
                    SlowTime.Instance.StopSlowMo();

                    m_Tutorial.HideTutorial(m_SlowMoAmount);
                }
            }
            m_SuccessfullyRiposted = true;
            SlowTime.Instance.m_PreventChanges = false;
            EventManager.StopListening("user_parry", OnUserFirstParry);

            if (m_HuntTimer != null)
                StopCoroutine(m_HuntTimer);
            return false;
        }

        dmg.OnBlockDamage();
        return true;
    }

    public void OnLungeStopInAir()
    {
        if (!m_SuccessfullyRiposted)
        {
            SlowTime.Instance.StartSlowMo(m_SlowMoAmount);
            SlowTime.Instance.m_PreventChanges = true;

            m_SlowMoTimer = SlowMoTimer();
            StartCoroutine(m_SlowMoTimer);

            m_Tutorial.ShowTutorial("B", "Parry", m_SlowMoAmount);
        }
    }

    public override void OnComboParried(AttackCombo combo)
    {
        base.OnComboParried(combo);
        if (!m_SuccessfullyRiposted)
        {
            if (m_SlowMoTimer != null)
                StopCoroutine(m_SlowMoTimer);

            m_SlowMoTimer = SlowMoTimer();
            StartCoroutine(m_SlowMoTimer);

            SlowTime.Instance.m_PreventChanges = false;
            SlowTime.Instance.StartSlowMo(m_SlowMoAmount);
            SlowTime.Instance.m_PreventChanges = true;

            m_Tutorial.ShowTutorial("X", "Riposte", m_SlowMoAmount);
        }

        if (m_HuntTimer == null)
            StopCoroutine(m_HuntTimer);
    }

    private void OnUserFirstParry()
    {
        m_PlayerControls.DisableAndLock(m_PlayerDashCommand);

        if (m_SlowMoTimer != null)
        {
            StopCoroutine(m_SlowMoTimer);
            SlowTime.Instance.m_PreventChanges = false;
            SlowTime.Instance.StopSlowMo();
            SlowTime.Instance.m_PreventChanges = true;
            m_Tutorial.HideTutorial();
        }
    }

    private IEnumerator SlowMoTimer()
    {
        yield return new WaitForSeconds(m_SlowMoTime * m_SlowMoAmount);
        SlowTime.Instance.m_PreventChanges = false;
        SlowTime.Instance.StopSlowMo();

        m_PlayerControls.EnableAndUnlock(m_PlayerDashCommand);
        m_PlayerControls.EnableAllCommands();
        m_Tutorial.HideTutorial(m_SlowMoAmount);
    }

    public override void CancelAndReset()
    {
        m_Active = false;
        m_JumpSoon = false;
        m_Tutorial.HideTutorial(1f);
        base.CancelAndReset();
    }

    protected override void OnJumpStart()
    {
        base.OnJumpStart();
        PlayLightAttackSound(true);
    }

    private void EndPhase()
    {
        m_Active = false;
        m_BossHealth.m_CurrentHealth = m_BossHealth.m_MaxHealth;

        m_PlayerAttackCommand.m_RegularHitDamage = m_AttackDmgAmountBefore;
        m_PlayerAttackCommand.m_FinalHitDamage = m_FinalDmgAmountBefore;
        m_PlayerAttackCommand.m_RiposteDamage = m_RiposteDmgAmountBefore;

        m_PlayerParryCommand.m_PerfectParryTime = m_PlayerParryCommand.m_PerfectParryTime - m_BlockTimeBefore;
        m_PlayerParryCommand.m_OkParryTime = m_BlockTimeBefore;

        m_JumpCombo.CancelCombo();
        m_LungeAttack.m_LungeAttackCallbacks = null;

        UnRegisterEventsForSound();

        WerewolfHittable hittable = FindObjectOfType<WerewolfHittable>();
        if (hittable != null)
            hittable.StopPlayingCriticalHPSound();

        if (m_Callbacks != null)
            m_Callbacks.PhaseEnd(this);
    }
}

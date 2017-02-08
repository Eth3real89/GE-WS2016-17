using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampirePhase0TutorialController : VampireController {

    public TutorialPromptController m_TutorialVisuals;

    public float m_TutorialSlowMo = 0.01f;

    public BlastWaveAttack m_BlastAttackForDashTutorial;
    public SwayingAEDamage m_BeamDamage;
    
    public Transform m_PlaceToBeAttacked;

    public CharacterHealth m_BossHealth;

    private const int m_DashTutorialBlast = 0;
    private const int m_DashTutorialBeam = 1;
    private const int m_ParryTutorialBullet = 2;
    private const int m_ParryDeflectTutorialBullet = 3;
    private const int m_EvasionTutorial = 4;
    private const int m_AttackTutorial = 5;

    private IEnumerator m_TutorialEnumerator;
    private IEnumerator m_PhaseEndEnumerator;

    private bool m_FirstDamageToBoss = false;

    private float m_HealthAtStartOfTutorial;

    public PlayerControls m_PlayerControls;
    public PlayerMoveCommand m_PlayerMove;
    public PlayerDashCommand m_PlayerDash;
    public PlayerHealCommand m_PlayerHeal;
    public PlayerAttackCommand m_PlayerAttack;
    public PlayerParryCommand m_PlayerParry;

    public override void StartPhase(BossfightCallbacks callbacks)
    {
        base.StartPhase(callbacks);
        StartCoroutine(StartAfterDelay());

        EventManager.StartListening(PlayerDashCommand.COMMAND_EVENT_TRIGGER, OnPlayerDash);
        EventManager.StartListening(PlayerParryCommand.COMMAND_EVENT_TRIGGER, OnPlayerParry);
    }

    protected override IEnumerator StartAfterDelay()
    {
        PlayerControls controls = FindObjectOfType<PlayerControls>();

        if (controls != null)
            controls.DisableAllCommands();

        yield return new WaitForSeconds(0.5f);

        GatherLight(2f);
        ActivateLightShield();

        yield return new WaitForSeconds(2f);

        if (controls != null)
            controls.EnableAllCommands();

        yield return base.StartAfterDelay();
    }

    public override void OnComboStart(AttackCombo combo)
    {
        if (m_TutorialEnumerator != null)
            StopCoroutine(m_TutorialEnumerator);

        base.OnComboStart(combo);
        m_HealthAtStartOfTutorial = GetScarletHealth();

        switch(m_CurrentComboIndex)
        {
            case m_DashTutorialBlast:
                m_TutorialEnumerator = DashTutorialBlastWave();
                break;
            case m_DashTutorialBeam:
                m_TutorialEnumerator = DashTutorialBeam();
                break;
            case m_ParryTutorialBullet:
                m_TutorialEnumerator = BlockTutorial(false);
                break;
            case m_ParryDeflectTutorialBullet:
                m_TutorialEnumerator = BlockTutorial(true);
                break;
        }

        StartCoroutine(m_TutorialEnumerator);
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        float health = GetScarletHealth();

        if (m_TutorialEnumerator != null)
            StopCoroutine(m_TutorialEnumerator);

        switch (m_CurrentComboIndex) // @todo: this is buggy - need a better solution!
        {
            case m_DashTutorialBlast:
            case m_DashTutorialBeam:
            case m_ParryTutorialBullet:
            case m_ParryDeflectTutorialBullet:
                if (health == m_HealthAtStartOfTutorial)
                {
                    m_NextComboTimer = StartNextComboAfter(combo.m_TimeAfterCombo);
                    StartCoroutine(m_NextComboTimer);
                }
                else
                {
                    m_CurrentComboIndex--;
                    m_NextComboTimer = StartNextComboAfter(combo.m_TimeAfterCombo);
                    StartCoroutine(m_NextComboTimer);
                }
                break;
            case m_EvasionTutorial:
                m_BossHittable.RegisterInterject(this);
                StartAttackTutorial();
                break;
        }
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        GatherLight(m_GatherLightTime);
        yield return new WaitForSeconds(m_GatherLightTime);

        StartCoroutine(PerfectTrackingRoutine(m_PerfectRotationTime));
        yield return new WaitForSeconds(m_PerfectRotationTime);

        yield return new WaitForSeconds(time);
        StartNextCombo();
    }

    private float GetScarletHealth()
    {
        return m_Scarlet.GetComponentInChildren<CharacterHealth>().m_CurrentHealth;
    }

    private IEnumerator DashTutorialBlastWave()
    {
        bool showTutorial = false;

        m_PlayerMove.StopMoving();
        m_PlayerControls.DisableAndLock(m_PlayerMove, m_PlayerHeal, m_PlayerAttack, m_PlayerParry);
        m_PlayerControls.transform.rotation = Quaternion.Euler(0, 90, 0);

        while(true)
        {
            float distanceToScarlet = Vector3.Distance(this.transform.position, m_Scarlet.transform.position);

            if (m_BlastAttackForDashTutorial.m_WaveSize + 2 >= distanceToScarlet && m_BlastAttackForDashTutorial.m_WaveSize < distanceToScarlet)
            {
                showTutorial = true;
                break;
            }

            yield return null;
        }

        if (showTutorial)
        {
            SlowTime.Instance.StartSlowMo(m_TutorialSlowMo);
            m_TutorialVisuals.ShowTutorial("A", "+ Right: Dash over Wave", m_TutorialSlowMo);

            float t = 0;
            while ((t += Time.deltaTime) < 6 * m_TutorialSlowMo)
            {
                yield return null;
            }

            SlowTime.Instance.StopSlowMo();
            m_TutorialVisuals.HideTutorial(1);
            m_PlayerControls.EnableAndUnlock(m_PlayerMove, m_PlayerHeal, m_PlayerAttack, m_PlayerParry);
        }
    }

    private IEnumerator DashTutorialBeam()
    {
        yield return new WaitForSeconds(1.5f);

        // @todo: fix this (only technical problem are the angles, but the code is ugly & what it does is kind of silly?)

       /* bool showTutorial = false;
        int tutorialDirection = -1;

        while (true)
        {
            if (GetScarletHealth() < m_HealthAtStartOfTutorial)
            {
                showTutorial = false;
                break;
            }

            float angleToScarlet = BossTurnCommand.CalculateAngleTowards(this.transform, m_Scarlet.transform);
            while (angleToScarlet < 0)
                angleToScarlet += 360;

            float currentBeamAngle = m_BeamDamage.GetCurrentAngle();
            while (currentBeamAngle < 0)
                currentBeamAngle += 360;

            print(angleToScarlet + " " + currentBeamAngle);

            if (currentBeamAngle < angleToScarlet && currentBeamAngle + 5 >= angleToScarlet)
            {
                tutorialDirection = 1;
                showTutorial = true;
                break;
            }
            else if (currentBeamAngle > angleToScarlet && currentBeamAngle - 5 <= angleToScarlet)
            {
                tutorialDirection = -1;
                showTutorial = true;
                break;
            }

            yield return null;
        }

        if (showTutorial)
        {
            SlowTime.Instance.StartSlowMo(m_TutorialSlowMo);
            m_TutorialVisuals.ShowTutorial("A", (tutorialDirection == 1)? "+ Diagonal Left Up: Dash over Wave" : "Diagonal Right Down: Dash over Wave", m_TutorialSlowMo);

            float t = 0;
            while ((t += Time.deltaTime) < 6 * m_TutorialSlowMo)
            {
                yield return null;
            }

            SlowTime.Instance.StopSlowMo();
            m_TutorialVisuals.HideTutorial(1);
        } */
    }


    private IEnumerator BlockTutorial(bool deflect)
    {
        bool showTutorial = false;
        Bullet b;
        while ((b = FindObjectOfType<Bullet>()) == null)
            yield return null;

        while (true)
        {
            if (b == null)
                break;

            float distanceToScarlet = Vector3.Distance(b.transform.position - new Vector3(0, b.transform.position.y, 0), m_Scarlet.transform.position - new Vector3(0, m_Scarlet.transform.position.y, 0));

            if (distanceToScarlet < 1)
            {
                showTutorial = true;
                break;
            }

            yield return null;
        }

        if (showTutorial)
        {
            SlowTime.Instance.StartSlowMo(m_TutorialSlowMo);
            m_TutorialVisuals.ShowTutorial("X", deflect? "Deflect Bullet" : "Block Bullet", m_TutorialSlowMo);

            float t = 0;
            while ((t += Time.deltaTime) < 6 * m_TutorialSlowMo)
            {
                yield return null;
            }

            SlowTime.Instance.StopSlowMo();
            m_TutorialVisuals.HideTutorial(1);
        }
    }

    private void StartAttackTutorial()
    {
        base.DashTo(m_PlaceToBeAttacked, 1.5f);

        m_PhaseEndEnumerator = AllowDamageThenMoveOnToNextPhase();
        StartCoroutine(m_PhaseEndEnumerator);

        m_TutorialEnumerator = PlayerAttackTutorial();
        StartCoroutine(m_TutorialEnumerator);
    }

    private IEnumerator PlayerAttackTutorial()
    {
        yield return new WaitForSeconds(2f);

        DeactivateLightShield();
        m_GatheringLight = true;
        m_VampireAnimator.SetTrigger("GatherLightTrigger");

        SlowTime.Instance.StartSlowMo(m_TutorialSlowMo);
        m_TutorialVisuals.ShowTutorial("Y", "Hit the Vampire while he gathers Light!", m_TutorialSlowMo);
        float t = 0;
        while((t += Time.deltaTime) < 4 * m_TutorialSlowMo && !Input.anyKeyDown)
        {
            yield return null;
        }
        SlowTime.Instance.StopSlowMo();
        m_TutorialVisuals.HideTutorial();

        while(m_BossHealth.m_CurrentHealth == m_BossHealth.m_HealthOld)
        {
            yield return null;
        }

        m_FirstDamageToBoss = true;

        SlowTime.Instance.StartSlowMo(m_TutorialSlowMo);
        m_TutorialVisuals.ShowTutorial("Y", "Press repeatedly for combo attacks", m_TutorialSlowMo);

        t = 0;
        while ((t += Time.deltaTime) < 4 * m_TutorialSlowMo && !Input.anyKeyDown)
        {
            yield return null;
        }
        SlowTime.Instance.StopSlowMo();
        m_TutorialVisuals.HideTutorial();
    }

    private IEnumerator AllowDamageThenMoveOnToNextPhase()
    {
        while(!m_FirstDamageToBoss)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4f);

        if (m_TutorialEnumerator != null)
        {
            StopCoroutine(m_TutorialEnumerator);
        }

        UnRegisterAnimationEvents();
        m_Callback.PhaseEnd(this);
    }

    private void OnPlayerDash()
    {
        if (m_CurrentComboIndex == m_DashTutorialBeam || m_CurrentComboIndex == m_DashTutorialBlast)
        {
            if (m_TutorialEnumerator != null)
            {
                StopCoroutine(m_TutorialEnumerator);
                SlowTime.Instance.StopSlowMo();
                m_TutorialVisuals.HideTutorial(1);
            }

            m_PlayerControls.EnableAndUnlock(m_PlayerMove, m_PlayerHeal, m_PlayerAttack, m_PlayerParry);
        }
    }

    private void OnPlayerParry()
    {
        if (m_CurrentComboIndex == m_ParryDeflectTutorialBullet || m_CurrentComboIndex == m_ParryTutorialBullet)
        {
            if (m_TutorialEnumerator != null)
            {
                StopCoroutine(m_TutorialEnumerator);
                SlowTime.Instance.StopSlowMo();
                m_TutorialVisuals.HideTutorial(1);
            }
        }
    }
}

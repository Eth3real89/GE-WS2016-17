using System;
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
    private const int m_ParryTutorialBullet = 1;
    private const int m_ParryDeflectTutorialBullet = 2;
    private const int m_BiggerAttackTutorial = 3;

    private bool m_DashTutorialOver;

    private IEnumerator m_TutorialEnumerator;
    private IEnumerator m_PhaseEndEnumerator;

    private bool m_AllowHit;

    private int m_HitCount;

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
        m_AllowHit = false;
        m_HitCount = 0;

        m_DashTutorialOver = false;

        m_PlayerMove.StopMoving();
        m_PlayerControls.DisableAndLock(m_PlayerMove, m_PlayerHeal, m_PlayerAttack, m_PlayerParry, m_PlayerDash);

        EventManager.StartListening(PlayerDashCommand.COMMAND_EVENT_TRIGGER, OnPlayerDash);
        EventManager.StartListening(PlayerParryCommand.COMMAND_EVENT_TRIGGER, OnPlayerParry);
        EventManager.StartListening(BlastWaveAttack.ATTACK_HIT_EVENT, OnBlastWaveHit);
        EventManager.StartListening(Bullet.BULLET_HIT_SCARLET_EVENT, OnBulletHit);
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

        yield return base.StartAfterDelay();
    }

    public override void OnComboStart(AttackCombo combo)
    {
        if (m_TutorialEnumerator != null)
            StopCoroutine(m_TutorialEnumerator);

        base.OnComboStart(combo);

        m_TutorialEnumerator = null;

        switch(m_CurrentComboIndex)
        {
            case m_DashTutorialBlast:
                m_TutorialEnumerator = DashTutorialBlastWave();
                break;
            case m_ParryTutorialBullet:
                m_TutorialEnumerator = BlockTutorial(false);
                break;
            case m_ParryDeflectTutorialBullet:
                m_TutorialEnumerator = BlockTutorial(true);
                break;
            case m_BiggerAttackTutorial:
                m_TutorialEnumerator = BulletWarning();
                break;
        }

        if (m_TutorialEnumerator != null)
            StartCoroutine(m_TutorialEnumerator);
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        m_ActiveCombo = null;

        if (m_TutorialEnumerator != null)
            StopCoroutine(m_TutorialEnumerator);

        switch (m_CurrentComboIndex)
        {
            case m_DashTutorialBlast:
                if (!m_DashTutorialOver)
                    break;
                EventManager.StopListening(BlastWaveAttack.ATTACK_HIT_EVENT, OnBlastWaveHit);
                StartAttackTutorial();

                break;
            case m_BiggerAttackTutorial:
                EndPhase();
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

    private void EndPhase()
    {
        UnRegisterAnimationEvents();

        StopAllCoroutines();
        m_Callback.PhaseEnd(this);

        m_PlayerControls.EnableAndUnlock(m_PlayerMove, m_PlayerHeal, m_PlayerAttack, m_PlayerParry, m_PlayerDash);

        EventManager.StopListening(PlayerDashCommand.COMMAND_EVENT_TRIGGER, OnPlayerDash);
        EventManager.StopListening(PlayerParryCommand.COMMAND_EVENT_TRIGGER, OnPlayerParry);
        EventManager.StopListening(BlastWaveAttack.ATTACK_HIT_EVENT, OnBlastWaveHit);
        EventManager.StopListening(Bullet.BULLET_HIT_SCARLET_EVENT, OnBulletHit);

        CancelHitBehaviours();
    }

    private float GetScarletHealth()
    {
        return m_Scarlet.GetComponentInChildren<CharacterHealth>().m_CurrentHealth;
    }

    private IEnumerator DashTutorialBlastWave()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        bool showTutorial = false;

        m_PlayerControls.transform.rotation = Quaternion.Euler(0, 90, 0);

        while (true)
        {
            float distanceToScarlet = Vector3.Distance(this.transform.position, m_Scarlet.transform.position);

            if (m_BlastAttackForDashTutorial.m_WaveSize + 1.5 >= distanceToScarlet && m_BlastAttackForDashTutorial.m_WaveSize < distanceToScarlet)
            {
                showTutorial = true;
                break;
            }

            yield return null;
        }

        if (showTutorial)
        {
            m_PlayerControls.EnableAndUnlock(m_PlayerDash);
            SlowTime.Instance.StartSlowMo(m_TutorialSlowMo);
            m_TutorialVisuals.ShowTutorial("A", "+ Right: Dash over Wave", m_TutorialSlowMo);

            float t = 0;
            while ((t += Time.deltaTime) < 6 * m_TutorialSlowMo)
            {
                yield return null;
            }

            SlowTime.Instance.StopSlowMo();
            m_TutorialVisuals.HideTutorial(1);
            m_PlayerControls.DisableAndLock(m_PlayerDash);
        }
    }

    private IEnumerator BlockTutorial(bool deflect)
    {
        m_PlayerControls.DisableAndLock(m_PlayerMove, m_PlayerHeal, m_PlayerAttack, m_PlayerParry, m_PlayerDash);

        bool showTutorial = false;
        Bullet b;
        while ((b = FindObjectOfType<Bullet>()) == null)
            yield return null;

        while (true)
        {
            if (b == null)
                break;

            float distanceToScarlet = Vector3.Distance(b.transform.position - new Vector3(0, b.transform.position.y, 0), m_Scarlet.transform.position - new Vector3(0, m_Scarlet.transform.position.y, 0));

            if (distanceToScarlet < 1.5)
            {
                showTutorial = true;
                break;
            }

            yield return null;
        }

        if (showTutorial)
        {
            m_PlayerControls.EnableAndUnlock(m_PlayerParry);
            SlowTime.Instance.StartSlowMo(m_TutorialSlowMo);
            m_TutorialVisuals.ShowTutorial("B", deflect? "Deflect Bullet" : "Block Bullet", m_TutorialSlowMo);

            float t = 0;
            while ((t += Time.deltaTime) < 6 * m_TutorialSlowMo)
            {
                yield return null;
            }

            SlowTime.Instance.StopSlowMo();
            m_TutorialVisuals.HideTutorial(1);
        }
        m_PlayerControls.DisableAndLock(m_PlayerParry);
    }

    private void StartAttackTutorial()
    {
        m_LightGuard.ReattachVisualsToParent();

        m_AllowHit = true;

        m_TutorialEnumerator = PlayerAttackTutorial();
        StartCoroutine(m_TutorialEnumerator);
    }

    private IEnumerator PlayerAttackTutorial()
    {
        m_PlayerControls.EnableAndUnlock(m_PlayerMove, m_PlayerDash, m_PlayerParry, m_PlayerAttack, m_PlayerHeal);
        m_BossHittable.RegisterInterject(this);

        DeactivateLightShield();
        yield return new WaitForSeconds(1.5f);

        DashTo(m_PlaceToBeAttacked, 1f);
        yield return new WaitForSeconds(1.3f);

        m_GatheringLight = true;
        m_AllowHit = true;
        m_VampireAnimator.ResetTrigger("StopGatherLightTrigger");
        m_VampireAnimator.SetTrigger("GatherLightTrigger");

        SlowTime.Instance.StartSlowMo(m_TutorialSlowMo);
        m_TutorialVisuals.ShowTutorial("X", "Hit the Vampire while he gathers Light!", m_TutorialSlowMo);
        float t = 0;
        while((t += Time.deltaTime) < 4 * m_TutorialSlowMo && !Input.anyKeyDown)
        {
            yield return null;
        }

        SlowTime.Instance.StopSlowMo();
        m_TutorialVisuals.HideTutorial();

        bool showTutorialAgain = true;
        t = 0;
        while((t += Time.deltaTime) < 10)
        {
            if (m_BossHealth.m_CurrentHealth == m_BossHealth.m_HealthStart)
            {
                yield return null;
                continue;
            }
           
            showTutorialAgain = false;
            break;
        }

        if (showTutorialAgain)
        {
            SlowTime.Instance.StartSlowMo(m_TutorialSlowMo);
            m_TutorialVisuals.ShowTutorial("X", "Hit the Vampire while he gathers Light!", m_TutorialSlowMo);
            t = 0;
            while ((t += Time.deltaTime) < 4 * m_TutorialSlowMo && !Input.anyKeyDown)
            {
                yield return null;
            }
            SlowTime.Instance.StopSlowMo();
            m_TutorialVisuals.HideTutorial();
        }

        while (m_BossHealth.m_CurrentHealth == m_BossHealth.m_HealthStart)
        {
            yield return null;
        }

        SlowTime.Instance.StartSlowMo(m_TutorialSlowMo);
        m_TutorialVisuals.ShowTutorial("X", "Press repeatedly for combo attacks", m_TutorialSlowMo);

        t = 0;
        while ((t += Time.deltaTime) < 4 * m_TutorialSlowMo && !Input.anyKeyDown)
        {
            yield return null;
        }
        SlowTime.Instance.StopSlowMo();
        m_TutorialVisuals.HideTutorial();

        yield return new WaitForSeconds(1.5f);

        m_AllowHit = false;
        StartCoroutine(BeforeParryTutorial());
    }

    private void OnPlayerDash()
    {
        if (m_CurrentComboIndex == m_DashTutorialBlast && m_ActiveCombo == m_Combos[0])
        {
            if (m_TutorialEnumerator != null)
            {
                StopCoroutine(m_TutorialEnumerator);
                SlowTime.Instance.StopSlowMo();
                m_TutorialVisuals.HideTutorial(1);
            }

            m_DashTutorialOver = true;

            m_PlayerControls.DisableAndLock(m_PlayerMove, m_PlayerHeal, m_PlayerAttack, m_PlayerParry, m_PlayerDash);
            StartCoroutine(MoveOnAfterWaiting());
        }
    }

    private void OnBlastWaveHit()
    {
        StartCoroutine(TryAgainAfterWaiting());
    }

    private void OnBulletHit()
    {
        m_ComboActive = false;
        m_ActiveCombo = null;

        if (m_CurrentComboIndex == m_ParryDeflectTutorialBullet || m_CurrentComboIndex == m_ParryTutorialBullet)
            StartCoroutine(DecideIfHitWasGood());
    }

    private IEnumerator DecideIfHitWasGood()
    {
        CharacterHealth h = m_Scarlet.GetComponent<CharacterHealth>();

        float health = h.m_CurrentHealth;
        
        if (m_ActiveCombo != null)
            m_ActiveCombo.CancelCombo();

        if (m_TutorialEnumerator != null)
            StopCoroutine(m_TutorialEnumerator);

        if (health != h.m_HealthOld)
        {
            // not good
            StartCoroutine(TryAgainAfterWaiting());
        }
        else
        {
            yield return new WaitForSeconds(0.2f);

            if (h.m_CurrentHealth != health)
            {
                // not good
                StartCoroutine(TryAgainAfterWaiting());
            }
            else
            {
                // good
                StartCoroutine(StartNextComboAfter(1f));

                if (m_CurrentComboIndex == m_ParryDeflectTutorialBullet)
                {
                    m_PlayerControls.EnableAndUnlock(m_PlayerMove, m_PlayerHeal, m_PlayerAttack, m_PlayerParry, m_PlayerDash);
                    EventManager.StopListening(Bullet.BULLET_HIT_SCARLET_EVENT, OnBulletHit);
                }
            }
        }
    }

    private IEnumerator BeforeParryTutorial()
    {
        DashTo(m_BetweenLightZones[0], 1f);
        yield return new WaitForSeconds(1.3f);

        m_LightGuard.m_ExpandLightGuardTime = 0.5f;
        ActivateLightShield();
        m_LightGuard.ReattachVisualsToParent();

        yield return new WaitForSeconds(0.5f);

        StartNextCombo();
    }

    private IEnumerator TryAgainAfterWaiting()
    {
        yield return new WaitForSeconds(2f);

        CancelComboIfActive();

        m_CurrentComboIndex--;
        m_NextComboTimer = StartNextComboAfter(0.5f);
        StartCoroutine(m_NextComboTimer);
    }

    private IEnumerator MoveOnAfterWaiting()
    {
        if (m_ActiveCombo != null)
            m_ActiveCombo.CancelCombo();
        yield return new WaitForSeconds(0.5f);

        OnComboEnd(m_ActiveCombo);
    }

    private IEnumerator BulletWarning()
    {
        yield return new WaitForSeconds(0.2f);

        SlowTime.Instance.StartSlowMo(m_TutorialSlowMo);
        m_TutorialVisuals.ShowTutorial("", "Some Bullets cannot be Blocked", m_TutorialSlowMo);

        float t = 0;
        while ((t += Time.deltaTime) < 8 * m_TutorialSlowMo && !Input.anyKeyDown)
        {
            yield return null;
        }
        SlowTime.Instance.StopSlowMo();
        m_TutorialVisuals.HideTutorial(1f);
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

    public override bool OnHit(Damage dmg)
    {
        if (dmg is BulletDamage)
        {
            return false;
        }

        if (m_AllowHit)
        {
            m_HitCount++;
            if (m_HitCount == 4)
            {
                StopAllCoroutines();
                UnRegisterAnimationEvents();
                SlowTime.Instance.StopSlowMo();

                if (m_TutorialEnumerator != null)
                    StopCoroutine(m_TutorialEnumerator);

                m_AllowHit = false;

                StartCoroutine(BeforeParryTutorial());
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    public override void CancelAndReset()
    {
        base.CancelAndReset();
        m_TutorialVisuals.HideTutorial(1f);
    }

    protected override void OnBulletAttackStart()
    {
        PlayAttackSound(true);
        base.OnBulletAttackStart();
    }

    protected override void OnBeamAttackStart()
    {
        PlayAttackSound(false);
        base.OnBeamAttackStart();
    }

    protected override void OnBlastWaveStart()
    {
        PlayAttackSound(true);
        base.OnBlastWaveStart();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampirePhase3Controller : VampireController
{

    public Transform m_ArenaCenter;
    public Transform m_ArenaTop;

    private bool m_Killable;

    public override void StartPhase(BossfightCallbacks callbacks)
    {
        base.StartPhase(callbacks);
        m_Killable = false;

        UnRegisterAnimationEvents();

        StartCoroutine(StartAfterDelay());
    }

    protected override IEnumerator StartAfterDelay()
    {
        ((VampireHittable)m_BossHittable).m_DontPlaySound = true;

        PlayerControls controls = FindObjectOfType<PlayerControls>();
        if (controls != null) 
        {
            controls.DisableAllCommands();
            controls.StopMoving();
        }

        StartCoroutine(SlowlyDrainHealth());

        DashTo(m_ArenaCenter, 0.5f);
        new FARQ().ClipName("vampire").Location(transform).StartTime(165).EndTime(171).Volume(1).Play();
        yield return new WaitForSeconds(0.5f);

        m_VampireAnimator.SetTrigger("GatherLightTrigger");
        m_LightGuard.m_ExpandLightGuardTime = 1.5f;

        ActivateLightShield();

        StartCoroutine(PerfectTrackingRoutine(0.5f));
        yield return new WaitForSeconds(1.5f);

        m_VampireAnimator.SetBool("RageModeActive", true);
        m_VampireAnimator.SetTrigger("RageModeTrigger");

        if (controls != null)
            controls.EnableAllCommands();

        yield return base.StartAfterDelay();
        
    }

    private IEnumerator SlowlyDrainHealth()
    {
        float t = 0;
        float totalTime = 63; // probably better if that would be based on some dynamic calculation... but it isn't :(
        CharacterHealth health = GetComponentInChildren<CharacterHealth>();
        while ((t += Time.deltaTime) < totalTime)
        {
            float h1 = health.m_CurrentHealth;

            if (health != null)
                health.m_CurrentHealth = Mathf.Lerp(health.m_MaxHealth, 10, t / totalTime);

            if (h1 >= 0.3 * health.m_MaxHealth && health.m_CurrentHealth < 0.3 * health.m_MaxHealth)
                new FARQ().ClipName("vampire").Location(transform).StartTime(101f).EndTime(117.855f).Volume(1).PlayUnlessPlaying();

            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            yield return null;
        }

        StartCoroutine(MakeVampireKillable());
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        m_ActiveCombo = null;
        m_ComboActive = false;

        switch (m_CurrentComboIndex)
        {
            case 0: // spiral bullet attack over
                StartCoroutine(StartNextComboAfter(1f));
                break;
            case 1: // Double beam attack over
                StartCoroutine(InitLastDesperateAttemptToKillScarlet());
                break;
            case 2: // "sea of bullets" over
                StartCoroutine(StartNextComboAfter(1f));
                break;
            case 3: // 6 waves over
                m_BossHittable.RegisterInterject(this);
                break;
        }
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        yield return new WaitForSeconds(time);

        if (m_CurrentComboIndex == 0)
        {
            new FARQ().ClipName("vampire").Location(transform).StartTime(171.5f).EndTime(179).Volume(1).Play();
        }

        StartNextCombo();
    }

    protected virtual IEnumerator InitLastDesperateAttemptToKillScarlet() // best method name ever
    {
        m_LightGuard.ReattachVisualsToParent();

        yield return new WaitForSeconds(0.5f);

        yield return RotateToDegrees(0.2f, 0);

        DashTo(m_ArenaTop, 0.5f);
        yield return new WaitForSeconds(0.5f);

        yield return RotateToDegrees(0.2f, 180);

        m_VampireAnimator.SetTrigger("DesperationTrigger");

        new FARQ().ClipName("vampire").Location(transform).StartTime(67.7f).EndTime(75.31f).Volume(1).Play();

        StartCoroutine(StartNextComboAfter(1f));   
    }
    
    protected virtual IEnumerator MakeVampireKillable()
    {
        m_Killable = true;
        m_BossHittable.RegisterInterject(this);
        m_VampireAnimator.SetTrigger("KillableTrigger");
        DeactivateLightShield();
        yield return null;
    }

    public override bool OnHit(Damage dmg)
    {
        if (m_Killable)
        {
            new FARQ().ClipName("vampire").Location(transform).StartTime(101f).EndTime(117.855f).Volume(1).StopIfPlaying();
            new FARQ().ClipName("vampire").Location(transform).StartTime(79).EndTime(96).Play();
            m_Killable = false;
            CancelHitBehaviours();

            m_Callback.PhaseEnd(this);

            return false;
        }

        return true;
    }

    protected virtual IEnumerator RotateToDegrees(float totalTime, float degrees)
    {
        float t = 0;
        while ((t += Time.deltaTime) < totalTime)
        {
            transform.rotation = Quaternion.Euler(0, Mathf.Lerp(transform.rotation.y, degrees, t / totalTime), 0);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, degrees, 0);
    }
}

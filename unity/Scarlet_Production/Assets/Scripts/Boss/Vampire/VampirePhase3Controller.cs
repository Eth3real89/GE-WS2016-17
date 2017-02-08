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

        StartCoroutine(StartAfterDelay());
    }

    protected override IEnumerator StartAfterDelay()
    {
        PlayerControls controls = FindObjectOfType<PlayerControls>();
        if (controls != null)
            controls.DisableAllCommands();


        DashTo(m_ArenaCenter, 0.5f);
        yield return new WaitForSeconds(0.5f);

        m_VampireAnimator.SetTrigger("GatherLightTrigger");
        m_LightGuard.m_LightGuardRadius = 1.5f;
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

    public override void OnComboEnd(AttackCombo combo)
    {
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
                StartCoroutine(MakeVampireKillable());
                break;
        }
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        yield return new WaitForSeconds(time);
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
        
        StartCoroutine(StartNextComboAfter(1f));   
    }
    
    protected virtual IEnumerator MakeVampireKillable()
    {
        m_Killable = true;
        m_VampireAnimator.SetTrigger("KillableTrigger");
        DeactivateLightShield();

        CharacterHealth health = GetComponentInChildren<CharacterHealth>();
        if (health != null)
            health.m_CurrentHealth = 10;

        yield return null;
    }

    public override bool OnHit(Damage dmg)
    {
        if (!m_Killable)
            return true;
        return false;
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

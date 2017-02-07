using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampirePhase3Controller : VampireController
{

    public Transform m_ArenaCenter;
    public Transform m_ArenaTop;

    public override void StartPhase(BossfightCallbacks callbacks)
    {
        base.StartPhase(callbacks);
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
        m_LightGuard.m_ExpandLightGuardTime = 0.5f;
        ActivateLightShield();

        StartCoroutine(PerfectTrackingRoutine(0.5f));
        yield return new WaitForSeconds(0.5f);
        m_VampireAnimator.SetBool("RageModeActive", true);
        m_VampireAnimator.SetTrigger("RageModeTrigger");

        if (controls != null)
            controls.EnableAllCommands();
        
        yield return base.StartAfterDelay();
    }
}

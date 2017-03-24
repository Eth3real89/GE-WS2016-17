using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyBossfightPhase2 : FairyBossfightPhase {

    public CharacterHealth m_ArmorHealth;
    public CharacterHealth m_AEFairyHealth;
    public Collider m_AEFairyCollider;

    public Animator m_ArmorAnimator;
    public GameObject m_Sword;
    public GameObject m_Shield;
    protected bool m_EndInitialized = false;

    public PlayerControls m_PlayerControls;

    public override void StartPhase(FairyPhaseCallbacks callbacks)
    {
        base.StartPhase(callbacks);
        m_EndInitialized = false;

        // just in case the game is started from phase 2:
        m_Sword.gameObject.SetActive(true);
        m_Shield.gameObject.SetActive(true);

        m_AEFairyController.ExpandLightGuard();
    }

    protected override void Update()
    {
        if (!m_Active)
            return;

        if (!m_EndInitialized && m_ArmorHealth.m_CurrentHealth <= 0)
            EndPhase();
    }

    protected override void EndPhase()
    {
        m_EndInitialized = true;
        EndCombo();

        m_AEFairyController.CancelAndReset();
        m_AEFairyController.StopAllCoroutines();
        m_ArmorFairyController.StopAllCoroutines();
        m_ArmorFairyController.ForceCancelHitBehaviours();

        m_AEFairyController.m_NotDeactivated = false;
        m_ArmorFairyController.m_NotDeactivated = false;
        m_ArmorFairyController.UnRegisterEventsForSound();

        m_AEFairyController.DisableLightGuard();

        m_ArmorAnimator.SetBool("Dead", true);
        m_ArmorAnimator.SetTrigger("DeathTriggerFront");

        StartCoroutine(RegenerateThenEnd());
    }

    protected virtual IEnumerator RegenerateThenEnd()
    {
        m_AEFairyCollider.isTrigger = false;
        m_AEFairyCollider.enabled = true;
        m_PlayerControls.DisableAllCommands();
        m_PlayerControls.StopMoving();

        m_Callback.SetPhaseIndicatorsEnabled(1);
        m_AEFairyController.ExpandLightGuard();
        yield return new WaitForSeconds(2f);

        m_AEFairyHealth.m_CurrentHealth = m_AEFairyHealth.m_MaxHealth;
        m_AEFairyHealth.transform.rotation = Quaternion.Euler(0, 0, 0);
        m_PlayerControls.EnableAllCommands();

        base.EndPhase();
    }
}

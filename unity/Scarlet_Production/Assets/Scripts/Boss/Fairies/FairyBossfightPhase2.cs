using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyBossfightPhase2 : FairyBossfightPhase {

    public CharacterHealth m_ArmorHealth;

    public Animator m_ArmorAnimator;
    public GameObject m_Sword;
    public GameObject m_Shield;

    public override void StartPhase(FairyPhaseCallbacks callbacks)
    {
        base.StartPhase(callbacks);

        // just in case the game is started from phase 2:
        m_Sword.gameObject.SetActive(true);
        m_Shield.gameObject.SetActive(true);
    }

    protected override void Update()
    {
        if (!m_Active)
            return;

        if (m_ArmorHealth.m_CurrentHealth <= 0)
            EndPhase();
    }

    protected override void EndPhase()
    {
        EndCombo();

        m_AEFairyController.StopAllCoroutines();
        m_ArmorFairyController.StopAllCoroutines();

        m_Active = false;
        StartCoroutine(PlayArmorDeath());
    }

    protected virtual IEnumerator PlayArmorDeath()
    {
        m_ArmorAnimator.SetTrigger("DeathTriggerFront");
        m_ArmorFairyController.m_BossHittable.RegisterInterject(null);
        yield return new WaitForSeconds(2f);


        base.EndPhase();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyBossfightPhase3 : FairyBossfightPhase {

    public CharacterHealth m_AEFairyHealth;

    public Animator m_ArmorAnimator;

    public override void StartPhase(FairyPhaseCallbacks callbacks)
    {
        m_Active = true;
        m_Callback = callbacks;

        m_AEFairyController.Initialize(this);

        m_Callback.OnPhaseStart(this);

        StartCombo();
    }

    public override void StartCombo()
    {
        m_AEFairyController.StartCombo(0);
    }

    protected override void Update()
    {
        if (!m_Active)
            return;

        if (m_AEFairyHealth.m_CurrentHealth < 0)
            EndPhase();
    }

    protected override void EndPhase()
    {
        EndCombo();

        m_AEFairyController.StopAllCoroutines();

        m_Active = false;
        StartCoroutine(ReanimateArmor());
    }

    protected virtual IEnumerator ReanimateArmor()
    {
        m_ArmorAnimator.SetTrigger("ReanimationTrigger");

        yield return new WaitForSeconds(2f);

        base.EndPhase();
    }
}

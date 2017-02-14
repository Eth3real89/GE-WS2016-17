using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyBossfightPhase2 : FairyBossfightPhase {

    public CharacterHealth m_ArmorHealth;

    protected override void Update()
    {
        if (!m_Active)
            return;

        if (m_ArmorHealth.m_CurrentHealth < 0)
            EndPhase();
    }

    protected override void EndPhase()
    {
        EndCombo();

        m_Active = false;
        m_ArmorHealth.m_CurrentHealth = m_ArmorHealth.m_MaxHealth;

        base.EndPhase();
    }
}

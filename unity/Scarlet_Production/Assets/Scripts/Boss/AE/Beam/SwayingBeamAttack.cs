using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayingBeamAttack : BeamAEAttack {

    public int m_NumSways;

    public override void OnExpansionOver(BeamAEDamage dmg)
    {
        if (m_Damage is SwayingAEDamage)
        {
            ((SwayingAEDamage)m_Damage).m_InitiallyAimAtScarlet = this.m_InitiallyAimAtScarlet;
            ((SwayingAEDamage)m_Damage).m_NumSways = this.m_NumSways;
        }
        base.OnExpansionOver(dmg);
    }

}

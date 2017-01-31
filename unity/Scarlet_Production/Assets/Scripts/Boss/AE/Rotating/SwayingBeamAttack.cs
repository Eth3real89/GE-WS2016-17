using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayingBeamAttack : RotatingAEAttack {

    public int m_NumSways;

    public override void OnExpansionOver()
    {
        if (m_Damage is SwayingAEDamage)
        {
            ((SwayingAEDamage)m_Damage).m_NumSways = this.m_NumSways;
        }
        base.OnExpansionOver();
    }

}

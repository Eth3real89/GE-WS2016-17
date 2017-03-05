using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayingBeamAttack : BeamAEAttack {

    public int m_NumSways;

    protected override void LoadPrefab()
    {
        m_Damage = AEPrefabManager.Instance.m_SwayingBeamWrapper.GetComponent<BeamAEDamage>();
        m_Damage.transform.parent = m_Container;
        m_Damage.transform.localPosition = new Vector3(0, 0, 0);
        m_Damage.transform.localRotation = Quaternion.Euler(0, 0, 0);
        m_Damage.transform.localScale = new Vector3(1, 1, 1);
    }

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

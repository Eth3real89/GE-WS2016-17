using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEAttack : BossAttack {

    public GameObject m_LightGuard;
    public bool m_GuardByLight = false;

    public override void StartAttack()
    {
        base.StartAttack();

        if (m_GuardByLight)
        {
            InitLightGuard();
        }
    }

    public override bool OnHit(Damage dmg)
    {
        dmg.OnBlockDamage();
        return true;
    }

    private void InitLightGuard()
    {
        m_LightGuard.SetActive(true);
        LightGuard guard = m_LightGuard.GetComponent<LightGuard>();
        if (guard != null)
            guard.Enable();
    }

    public void HideLightGuard()
    {
        if (m_LightGuard != null)   
            m_LightGuard.SetActive(false);
    }

    public override void CancelAttack()
    {
    }

}

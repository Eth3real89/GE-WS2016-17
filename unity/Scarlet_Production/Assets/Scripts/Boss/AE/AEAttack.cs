using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEAttack : BossAttack {

    public GameObject m_LightGuard;

    public BlastWaveVisuals m_LightGuardVisuals;

    public bool m_GuardByLight = false;
    public float m_LightGuardRadius = 0;
    public float m_ExpandLightGuardTime = 0.01f;

    private IEnumerator m_LightGuardEnumerator;

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
        if (m_GuardByLight)
        {
            dmg.OnBlockDamage();
            return false;
        }

        return base.OnHit(dmg);
    }

    private void InitLightGuard()
    {
        m_LightGuard.SetActive(true);

        m_LightGuardEnumerator = GrowLightGuard();
        StartCoroutine(m_LightGuardEnumerator);
    }

    private IEnumerator GrowLightGuard()
    {
        float t = 0;

        m_LightGuardVisuals.Setup();

        while((t += Time.deltaTime) < m_ExpandLightGuardTime)
        {
            m_LightGuardVisuals.ScaleUp(t / m_ExpandLightGuardTime * m_LightGuardRadius);
            yield return null;
        }

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

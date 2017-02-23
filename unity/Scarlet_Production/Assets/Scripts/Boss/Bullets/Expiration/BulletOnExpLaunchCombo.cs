using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletOnExpLaunchCombo : BulletOnExpireBehaviour, AttackCombo.ComboCallback
{
    public AttackCombo m_Combo;
    public GameObject m_LeftOverEmptyObject;

    public override void OnBulletExpires(BulletBehaviour b)
    {
        GameObject empty = GameObject.Instantiate(m_LeftOverEmptyObject);
        empty.transform.position = new Vector3(b.transform.position.x, b.transform.position.y, b.transform.position.z);

        foreach(BossAttack attack in m_Combo.m_Attacks)
        {
            if (attack is BlastWaveAttack)
            {
                ((BlastWaveAttack)attack).m_Center = empty.transform;
            }
            else if (attack is BulletAttack)
            {
                ((BulletAttack)attack).m_BaseSwarm.m_Invoker.m_Base = empty.transform;
            }
            else if (attack is LightGuardAttack)
            {
                ((LightGuardAttack)attack).m_LightGuard.m_Center = empty;
            }
        }


        m_Combo.m_Callback = this;
        m_Combo.LaunchCombo();
    }

    public void OnComboEnd(AttackCombo combo)
    {
    }

    public void OnComboParried(AttackCombo combo)
    {
    }

    public void OnComboStart(AttackCombo combo)
    {
    }

    public void OnInterruptCombo(AttackCombo combo)
    {
    }
}

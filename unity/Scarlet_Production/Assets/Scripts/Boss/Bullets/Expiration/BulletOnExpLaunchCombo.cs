using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletOnExpLaunchCombo : BulletOnExpireBehaviour, AttackCombo.ComboCallback
{
    public AttackCombo m_Combo;
    public GameObject m_LeftOverEmptyObject;

    protected bool m_Destroy;

    public override void OnBulletExpires(BulletBehaviour b)
    {
        m_Destroy = false;

        GameObject empty = GameObject.Instantiate(m_LeftOverEmptyObject);
        empty.transform.position = new Vector3(b.transform.position.x, b.transform.position.y, b.transform.position.z);
        empty.transform.rotation = new Quaternion(b.transform.rotation.x, b.transform.rotation.y, b.transform.rotation.z, b.transform.rotation.w);

        foreach (BossAttack attack in m_Combo.m_Attacks)
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

        m_Combo = GameObject.Instantiate(m_Combo);
        
        m_Combo.m_Callback = this;
        m_Combo.LaunchCombo();

        StartCoroutine(DestroyOnFinish(empty, m_Combo.gameObject));
    }

    public void OnComboEnd(AttackCombo combo)
    {
        m_Destroy = true;
    }

    public void OnComboParried(AttackCombo combo)
    {
    }

    public void OnComboStart(AttackCombo combo)
    {
    }

    protected virtual IEnumerator DestroyOnFinish(GameObject go, GameObject comboCopy)
    {
        while(!m_Destroy)
        {
            yield return null;
        }
        // just to be safe
        yield return new WaitForSeconds(5f);

        if (go != null)
            GameObject.Destroy(go);

        if (comboCopy != null)
            GameObject.Destroy(comboCopy);
    }

    public void OnInterruptCombo(AttackCombo combo)
    {
    }
}

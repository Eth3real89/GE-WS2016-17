using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletOnExpLaunchCombo : BulletOnExpireBehaviour, AttackCombo.ComboCallback
{
    public AttackCombo m_Combo;
    public GameObject m_LeftOverEmptyObject;

    public bool m_DontCopy = false;
    public bool m_LowerBullets = false;

    protected bool m_Destroy;

    /// <summary>
    /// Use with extreme caution!
    /// </summary>
    public bool m_AbortOnExpiration = false;

    public override void OnBulletExpires(BulletBehaviour b)
    {
        if (m_AbortOnExpiration)
        {
            m_AbortOnExpiration = false;
            return;
        }

        m_Destroy = false;

        GameObject empty = GameObject.Instantiate(m_LeftOverEmptyObject);
        empty.transform.position = new Vector3(b.transform.position.x, b.transform.position.y + 0.5f, b.transform.position.z);
        empty.transform.rotation = new Quaternion(b.transform.rotation.x, b.transform.rotation.y, b.transform.rotation.z, b.transform.rotation.w);

        GameObject emptyLower = GameObject.Instantiate(m_LeftOverEmptyObject);
        emptyLower.transform.position = new Vector3(b.transform.position.x, b.transform.position.y, b.transform.position.z);
        emptyLower.transform.rotation = new Quaternion(b.transform.rotation.x, b.transform.rotation.y, b.transform.rotation.z, b.transform.rotation.w);

        foreach (BossAttack attack in m_Combo.m_Attacks)
        {
            if (attack is BlastWaveAttack)
            {
                ((BlastWaveAttack)attack).m_Center = empty.transform;
                ((BlastWaveAttack)attack).m_KillVisualsWhenOver = true;
            }
            else if (attack is BulletAttack)
            {
                ((BulletAttack)attack).m_BaseSwarm.m_Invoker.m_Base = m_LowerBullets? emptyLower.transform : empty.transform;
            }
            else if (attack is LightGuardAttack)
            {
                ((LightGuardAttack)attack).m_LightGuard.m_Center = empty;
            }
            else if (attack is PlayableAttack)
            {
                ((PlayableAttack)attack).m_EffectLocation = emptyLower.transform.position;
            }
        }

        if (!m_DontCopy)
            m_Combo = GameObject.Instantiate(m_Combo);
        
        m_Combo.m_Callback = this;
        m_Combo.LaunchCombo();

        if (!m_DontCopy)
            StartCoroutine(DestroyOnFinish(empty, m_Combo.gameObject, emptyLower));
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

    protected virtual IEnumerator DestroyOnFinish(params GameObject[] gos)
    {
        while(!m_Destroy)
        {
            yield return null;
        }
        // just to be safe
        yield return new WaitForSeconds(5f);

        for(int i = 0; i < gos.Length; i++)
        {
            if (gos[i] != null)
                Destroy(gos[i]);
        }

        if (gameObject != null)
            Destroy(gameObject);
    }

    public void OnInterruptCombo(AttackCombo combo)
    {
    }
}

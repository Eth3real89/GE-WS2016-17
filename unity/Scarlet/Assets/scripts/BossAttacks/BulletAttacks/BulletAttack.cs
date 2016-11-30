using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletAttack : Attack {

    public List<Bullet> m_Bullets;
    protected MonoBehaviour m_MonoBehaviour;

    public BulletAttack(MonoBehaviour monoBehaviour)
    {
        this.m_MonoBehaviour = monoBehaviour;
        m_Bullets = new List<Bullet>();
    }

    public override void WhileActive()
    {
        List<int> removeIndices = new List<int>();

        for(int i = 0; i < m_Bullets.Count; i++)
        {
            if (m_Bullets[i] == null)
                removeIndices.Add(i);
            else if (m_Bullets[i].m_Destroy)
            {
                removeIndices.Add(i);
                m_Bullets[i].DestroyBullet();
            }
        }

        for(int i = removeIndices.Count - 1; i >= 0; i--)
        {
            m_Bullets.RemoveAt(i);
        }
    }
    
}

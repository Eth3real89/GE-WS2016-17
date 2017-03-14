using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletConeFactoryInvoker : BulletFactoryInvoker {

    public int[] m_IterationBulletCounts;

    public float m_Angle;
    public float m_InitialDistance;
    
    protected override void SpawnIteration(BulletSwarm bs, IEnumerator onFinish = null)
    {
        int count = m_IterationBulletCounts[m_CurrentIteration];

        float angleStep = (count > 1)? ((m_Angle) / ((m_Angle == 360)? count : count - 1)) : 0;

        for(int i = 0; i < m_IterationBulletCounts[m_CurrentIteration]; i++)
        {
            BulletBehaviour b = m_Factories[m_CurrentIteration].CreateBullet();
            b.transform.position = m_Base.transform.position;
            b.transform.rotation = m_Base.transform.rotation;

            b.Launch(bs);

            float angle = angleStep * i - m_Angle / 2;
            if (count == 1)
                angle = 0;

            b.transform.rotation = Quaternion.Euler(b.transform.rotation.eulerAngles + new Vector3(0, angle, 0));
            b.transform.position += b.transform.forward * m_InitialDistance;

            bs.m_Instances.Add(b);
        }

        m_CurrentIteration++;
        if (m_CurrentIteration >= m_IterationBulletCounts.Length)
        {
            if (onFinish != null)
            {
                StartCoroutine(onFinish);
            }

            EventManager.TriggerEvent(BulletAttack.END_EVENT_NAME);
            return;
        }

        m_LaunchIterationTimer = BetweenIterations(bs, onFinish);
        StartCoroutine(m_LaunchIterationTimer);
    }

}

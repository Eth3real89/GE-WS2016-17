using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFactoryInvoker : MonoBehaviour {

    public BulletFactory[] m_Factories;
    public Transform m_Base;

    public int m_Iterations;
    public float m_TimeBetweenIterations;

    protected int m_CurrentIteration;

    protected IEnumerator m_LaunchIterationTimer;

    public virtual void Launch(BulletSwarm bs)
    {
        m_CurrentIteration = 0;
        SpawnIteration(bs);
    }

    protected virtual void SpawnIteration(BulletSwarm bs)
    {
        if (m_CurrentIteration >= m_Iterations)
            return;

        for (int i = 0; i < m_Factories.Length; i++)
        {
            BulletBehaviour b = CreateBullet(i);
            b.Launch(bs);
            bs.m_Instances.Add(b);
        }

        m_CurrentIteration++;

        m_LaunchIterationTimer = BetweenIterations(bs);
        StartCoroutine(m_LaunchIterationTimer);
    }

    protected virtual BulletBehaviour CreateBullet(int factoryIndex)
    {
        BulletBehaviour b = m_Factories[factoryIndex].CreateBullet();

        b.transform.position = m_Base.transform.position;
        b.transform.rotation = m_Base.transform.rotation;
        return b;
    }

    protected virtual IEnumerator BetweenIterations(BulletSwarm bs)
    {
        yield return new WaitForSeconds(m_TimeBetweenIterations);

        SpawnIteration(bs);
    }

}

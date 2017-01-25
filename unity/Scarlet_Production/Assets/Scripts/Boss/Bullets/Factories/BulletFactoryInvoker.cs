using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFactoryInvoker : MonoBehaviour {


    public BulletFactory[] m_Factories;

    public int m_Iterations;
    public float m_TimeBetweenIterations;

    private int m_CurrentIteration;

    private IEnumerator m_LaunchIterationTimer;

    public void Launch(BulletSwarm bs)
    {
        m_CurrentIteration = 0;
        SpawnIteration(bs);
    }

    private void SpawnIteration(BulletSwarm bs)
    {
        if (m_CurrentIteration >= m_Iterations)
            return;

        for (int i = 0; i < m_Factories.Length; i++)
        {
            BulletBehaviour b = m_Factories[i].CreateBullet();
            b.Launch();

            bs.m_Instances.Add(b);
        }

        m_CurrentIteration++;

        m_LaunchIterationTimer = BetweenIterations(bs);
        StartCoroutine(m_LaunchIterationTimer);
    }

    private IEnumerator BetweenIterations(BulletSwarm bs)
    {
        yield return new WaitForSeconds(m_TimeBetweenIterations);

        SpawnIteration(bs);
    }

}

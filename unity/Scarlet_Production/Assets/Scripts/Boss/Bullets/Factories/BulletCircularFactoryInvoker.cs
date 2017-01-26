using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCircularFactoryInvoker : BulletFactoryInvoker {

    public float m_Radius;
    public float m_TimeToGetInFormation;

    public float m_TimeToSpawnBullets;
    private float m_Time;
    private float m_AbsoluteStartTime;

    private Vector3 m_Center;

    public BulletCircularMovement m_FirstMovement;

    protected override void SpawnIteration(BulletSwarm bs)
    {
        if (m_CurrentIteration >= m_Factories.Length)
            return;

        if (m_CurrentIteration == 0)
        {
            m_Time = 0;
            m_AbsoluteStartTime = Time.timeSinceLevelLoad;
        }
        else
        {
            m_Time = Time.timeSinceLevelLoad - m_AbsoluteStartTime;
        }

        BulletBehaviour b = CreateBullet(m_CurrentIteration);
        b.transform.rotation = Quaternion.Euler(m_Base.transform.rotation.eulerAngles);
        b.transform.position = new Vector3(m_Base.transform.position.x, m_Base.transform.position.y, m_Base.transform.position.z);
        

        b.Launch(bs);
        bs.m_Instances.Add(b);

        m_CurrentIteration++;

        m_LaunchIterationTimer = BetweenIterations(bs);
        StartCoroutine(m_LaunchIterationTimer);
    }

    protected override BulletBehaviour CreateBullet(int factoryIndex)
    {
        BulletBehaviour b = base.CreateBullet(factoryIndex);

        BulletMovement m = b.GetComponentInChildren<BulletMovement>();
        if (m is BulletCircularMovement)
        {
            BulletCircularMovement bcm = (BulletCircularMovement)m;
            bcm.m_IndexInCircle = factoryIndex;
            bcm.m_TotalBulletsInCircle = m_Factories.Length;
            bcm.m_Radius = m_Radius;
            bcm.m_TimeToGetInFormation = m_TimeToGetInFormation - m_Time;

            if (factoryIndex == 0)
            {
                m_Center = bcm.CalculateCenter(b);
                m_FirstMovement = bcm;
            }
            else
            {
                bcm.m_Center = m_Center;
                bcm.SetCenterOffset(m_FirstMovement.CalculateCenterOffset());
            }

        }

        return b;
    }

    protected override IEnumerator BetweenIterations(BulletSwarm bs)
    {
        yield return new WaitForSeconds(m_TimeToSpawnBullets / (m_Factories.Length + 1));
        SpawnIteration(bs);
    }

}

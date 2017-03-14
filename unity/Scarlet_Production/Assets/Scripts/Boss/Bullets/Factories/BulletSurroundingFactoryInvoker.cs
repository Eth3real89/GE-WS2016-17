using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSurroundingFactoryInvoker : BulletFactoryInvoker {

    public float m_Radius;
    public float m_TimeToGetInFormation;

    public float m_TimeToSpawnBullets;
    private float m_Time;
    private float m_AbsoluteStartTime;

    public Transform m_Center;
    private Vector3 m_OffsetFix;

    public BulletCircularMovement m_FirstMovement;
    

    protected override void SpawnIteration(BulletSwarm bs, IEnumerator onFinish = null)
    {
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
        if (m_CurrentIteration >= m_Factories.Length)
        {
            if (onFinish != null)
                StartCoroutine(onFinish);
            EventManager.TriggerEvent(BulletAttack.END_EVENT_NAME);
            return;
        }

        m_LaunchIterationTimer = BetweenIterations(bs, onFinish);
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
                bcm.CalculateCenter(b);
                m_OffsetFix = m_Center.position - bcm.m_Center;
                m_OffsetFix.y = 0;
                bcm.SetCenterOffset(m_OffsetFix);

                m_FirstMovement = bcm;
            }
            else
            {
                bcm.m_Center = m_FirstMovement.m_Center;
                bcm.SetCenterOffset(m_FirstMovement.CalculateCenterOffset() + m_OffsetFix);
            }

        }

        return b;
    }

    protected override IEnumerator BetweenIterations(BulletSwarm bs, IEnumerator onFinish = null)
    {
        yield return new WaitForSeconds(m_TimeToSpawnBullets / (m_Factories.Length + 1));
        SpawnIteration(bs, onFinish);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Requires a Factory with a Multi Stage Movement with 3 entries:
///     1) Straight Movement
///     2) No Movement
///     3) 3rd movement: whatever you want then
/// </summary>
public class BulletCircleFactoryInvoker : BulletFactoryInvoker {

    public Transform m_Spawn;
    public float m_DistanceFromSpawnAtStart;

    public float m_TimeToReachBorders;
    public float m_TimeStandStill;

    public Transform m_Center;
    public float m_DistanceFromCenter;
    public int m_CountPerIteration;
    
    protected override void SpawnIteration(BulletSwarm bs, IEnumerator onFinish = null)
    {
        bs.transform.position = new Vector3(m_Center.position.x, m_Center.position.y, m_Center.position.z);

        for(int i = 0; i < m_CountPerIteration; i++)
        {
            BulletBehaviour b = CreateBullet(m_CurrentIteration, i);

            b.Launch(bs);
            bs.m_Instances.Add(b);
        }

        m_CurrentIteration++;
        if (m_CurrentIteration >= m_Iterations)
        {
            if (onFinish != null)
                StartCoroutine(onFinish);

            EventManager.TriggerEvent(BulletAttack.END_EVENT_NAME);
            return;
        }

        m_LaunchIterationTimer = BetweenIterations(bs, onFinish);
        StartCoroutine(m_LaunchIterationTimer);
        
    }

    protected virtual BulletBehaviour CreateBullet(int factoryIndex, int indexInCircle)
    {
        BulletBehaviour b = m_Factories[factoryIndex].CreateBullet();

        b.transform.position = m_Spawn.transform.position;
        b.transform.rotation = m_Spawn.transform.rotation;

        float bulletAngle = (indexInCircle / (float) m_CountPerIteration) * 360f;

        b.transform.Rotate(Vector3.up, bulletAngle);

        Vector3 goal = m_Center.position + Quaternion.Euler(0, bulletAngle, 0) * new Vector3(0, 0, 1) * m_DistanceFromCenter;
        
        goal.y = b.transform.position.y;
        b.transform.LookAt(goal);

        b.transform.position += b.transform.forward.normalized * m_DistanceFromSpawnAtStart;

        ((BulletStraightMovement)((BulletMultiStageMovement)b.m_Movement).m_Movements[0]).m_Speed = Vector3.Distance(b.transform.position, goal) / m_TimeToReachBorders;

        ((BulletMultiStageMovement)b.m_Movement).m_Times[0] = m_TimeToReachBorders;
        ((BulletMultiStageMovement)b.m_Movement).m_Times[1] = m_TimeStandStill;

        StartCoroutine(RotateBullet(b, bulletAngle - 180));

        return b;
    }

    private IEnumerator RotateBullet(BulletBehaviour b, float rotation)
    {
        yield return new WaitForSeconds(m_TimeToReachBorders);

        if (b != null && !b.m_KillBullet && b.gameObject != null)
        {
            b.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
    }

}

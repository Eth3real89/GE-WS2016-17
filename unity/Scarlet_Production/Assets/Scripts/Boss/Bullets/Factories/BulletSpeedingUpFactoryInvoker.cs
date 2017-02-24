using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpeedingUpFactoryInvoker : BulletFactoryInvoker {

    public float m_TimeWhenAllBulletsHaveEqualDistance;

    protected float m_TotalSpawnTime;

    protected override BulletBehaviour CreateBullet(int factoryIndex)
    {
        BulletBehaviour b = base.CreateBullet(factoryIndex);

        float timeSinceFirstBullet = m_CurrentIteration * m_TimeBetweenIterations;
        float timeLeft = m_TimeWhenAllBulletsHaveEqualDistance - timeSinceFirstBullet;

        if (b.m_Movement is BulletStraightMovement)
        {
            float distanceFirstBulletWouldHaveAtTime = ((BulletStraightMovement)b.m_Movement).m_Speed * m_TimeWhenAllBulletsHaveEqualDistance;
            ((BulletStraightMovement)b.m_Movement).m_Speed = distanceFirstBulletWouldHaveAtTime / timeLeft;
        }
        else if (b.m_Movement is BulletHomingMovement)
        {
            float distanceFirstBulletWouldHaveAtTime = ((BulletHomingMovement)b.m_Movement).m_Speed * m_TimeWhenAllBulletsHaveEqualDistance;
            ((BulletHomingMovement)b.m_Movement).m_Speed = distanceFirstBulletWouldHaveAtTime / timeLeft;
        }

        if (b.m_Expiration is BulletTimeBasedExpiration)
        {
            ((BulletTimeBasedExpiration)b.m_Expiration).m_Time -= timeSinceFirstBullet;
        }

        return b;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttack : BossAttack, BulletBehaviour.BulletCallbacks {

    public BulletSwarm m_BaseSwarm;

    public override void StartAttack()
    {
        base.StartAttack();

        m_BaseSwarm.Launch(this);
    }

    public override void CancelAttack()
    {
        if (m_BaseSwarm != null)
            m_BaseSwarm.Kill();
    }

    public void OnBulletCreated(BulletBehaviour bullet)
    {
    }

    public void OnBulletHitTarget(BulletBehaviour bullet)
    {
    }

    public void OnBulletParried(BulletBehaviour bullet)
    {
    }

    public void OnBulletDestroyed(BulletBehaviour bullet)
    {
    }
}

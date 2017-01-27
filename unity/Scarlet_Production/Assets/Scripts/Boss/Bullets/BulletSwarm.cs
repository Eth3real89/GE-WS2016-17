using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSwarm : BulletBehaviour, BulletBehaviour.BulletCallbacks {

    public List<BulletBehaviour> m_Instances;

    public BulletFactoryInvoker m_Invoker;

    public override void Launch(BulletCallbacks callbacks)
    {
        this.m_BulletCallbacks = callbacks;
        base.Launch(this);

        m_Instances = new List<BulletBehaviour>();
        m_Invoker.Launch(this);
    }


    public override void MoveBy(Vector3 movement)
    {
        transform.position += movement;

        foreach (BulletBehaviour b in m_Instances)
        {
            b.MoveBy(movement);
        }
    }

    public override void Kill()
    {
        m_KillBullet = true;
        foreach (BulletBehaviour b in m_Instances)
        {
            b.Kill();
        }
    }

    public void OnBulletCreated(BulletBehaviour bullet)
    {
    }

    public void OnBulletHitTarget(BulletBehaviour bullet, GameObject scarlet)
    {
        if (m_Instances.Contains(bullet))
            m_Instances.Remove(bullet);

        bullet.Kill();

        if (m_Instances.Count == 0)
            Kill();
    }

    public void OnBulletParried(BulletBehaviour bullet)
    {
    }

    public void OnBulletDestroyed(BulletBehaviour bullet)
    {
        if (m_Instances.Contains(bullet))
            m_Instances.Remove(bullet);

        if (m_Instances.Count == 0)
            Kill();
    }
}

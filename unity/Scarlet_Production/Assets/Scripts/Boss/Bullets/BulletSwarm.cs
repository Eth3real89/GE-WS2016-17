using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSwarm : BulletBehaviour, BulletBehaviour.BulletCallbacks {

    public List<BulletBehaviour> m_Instances;

    public BulletFactoryInvoker m_Invoker;

    public bool m_KillAllChildren = false; // category: variable names that sound really, really wrong
    public bool m_KillChildrenOnCancel = true;

    protected bool m_CancelIfIsCopy = false;

    protected bool m_Launched = false;

    public override void Launch(BulletCallbacks callbacks)
    {
        this.m_BulletCallbacks = callbacks;
        base.Launch(this);
        m_CancelIfIsCopy = false;

        if (!m_Launched)
        {
            m_Launched = true;
            m_Movement = Instantiate(m_Movement);
        }

        m_Instances = new List<BulletBehaviour>();
        m_Invoker.Launch(this, OnInvokerFinish());
    }

    protected virtual IEnumerator OnInvokerFinish()
    {
        yield return null;
        m_CancelIfIsCopy = true;
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
        MLog.Log(LogType.AELog, "Killing Bullet Swarm: " + this);

        if (m_Invoker != null)
            m_Invoker.Cancel();

        m_KillBullet = true;

        if (m_KillAllChildren && m_KillChildrenOnCancel)
        {
            foreach (BulletBehaviour b in m_Instances)
            {
                b.Kill();
            }
        }

        if (m_Movement != null)
        {
            Destroy(m_Movement.gameObject);
        }

        if (this != null && this.gameObject != null)
            Destroy(this.gameObject);
                
    }

    public void OnBulletCreated(BulletBehaviour bullet)
    {
    }

    public void OnBulletHitTarget(BulletBehaviour bullet)
    {
        if (m_Instances.Contains(bullet))
            m_Instances.Remove(bullet);

        bullet.Kill();

        CheckKill();
    }

    public void OnBulletParried(BulletBehaviour bullet)
    {

    }

    public void OnBulletDestroyed(BulletBehaviour bullet)
    {
        if (m_Instances.Contains(bullet))
            m_Instances.Remove(bullet);
        CheckKill();
    }

    public void LoseBullet(Bullet bullet)
    {
        if (m_Instances.Contains(bullet))
            m_Instances.Remove(bullet);
        CheckKill();
    }

    private void CheckKill()
    {
        if (m_Instances.Count == 0 && (m_Expiration is BulletNoExpiration || (transform.GetComponentInParent<BossHittable>() == null && m_CancelIfIsCopy)))
            Kill();
    }
}

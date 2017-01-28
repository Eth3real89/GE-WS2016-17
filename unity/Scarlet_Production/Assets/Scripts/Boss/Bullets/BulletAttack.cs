using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttack : BossAttack, BulletBehaviour.BulletCallbacks {

    public BulletSwarm m_BaseSwarm;
    private BulletSwarm m_ActiveCopy;

    public float m_MoveOnAfter = 1f;
    protected IEnumerator m_Timer;

    public override void StartAttack()
    {
        base.StartAttack();

        m_ActiveCopy = Instantiate(m_BaseSwarm);
        m_ActiveCopy.transform.position = m_BaseSwarm.transform.position;
        m_ActiveCopy.transform.rotation = m_BaseSwarm.transform.rotation;
        m_ActiveCopy.Launch(this);

        m_Timer = MoveOnTimer();
        StartCoroutine(m_Timer);
    }

    protected virtual IEnumerator MoveOnTimer()
    {
        yield return new WaitForSeconds(m_MoveOnAfter);
        m_Callback.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        if (m_ActiveCopy != null)
            m_ActiveCopy.Kill();
    }

    public void OnBulletCreated(BulletBehaviour bullet)
    {
    }

    public void OnBulletHitTarget(BulletBehaviour bullet, GameObject scarlet)
    {
    }

    public void OnBulletParried(BulletBehaviour bullet)
    {
    }

    public void OnBulletDestroyed(BulletBehaviour bullet)
    {
    }
}

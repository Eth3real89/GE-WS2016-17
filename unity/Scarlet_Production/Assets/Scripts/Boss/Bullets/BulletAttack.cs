using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAttack : BossAttack, BulletBehaviour.BulletCallbacks {

    public static string START_EVENT_NAME = "bullet_attack_start";
    public static string END_EVENT_NAME = "bullet_attack_end";

    public BulletSwarm m_BaseSwarm;
    private BulletSwarm m_ActiveCopy;

    public float m_AnimationStartTime = 0.15f;
    public float m_MoveOnAfter = 1f;
    protected IEnumerator m_Timer;

    public override void StartAttack()
    {
        base.StartAttack();

        EventManager.TriggerEvent(START_EVENT_NAME);

        m_Timer = StartAfterShortTime();
        StartCoroutine(m_Timer);
    }

    protected virtual IEnumerator StartAfterShortTime()
    {
        yield return new WaitForSeconds(m_AnimationStartTime);

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
        if (m_Timer != null)
            StopCoroutine(m_Timer);

        if (m_ActiveCopy != null)
            m_ActiveCopy.Kill();
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

    public void LoseBullet(Bullet bullet)
    {
    }
}

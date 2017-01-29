using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeBasedExpiration : BulletExpirationBehaviour
{

    public float m_Time;
    private IEnumerator m_Timer;

    private BulletBehaviour m_Bullet;

    public override void OnLaunch(BulletBehaviour b)
    {
        m_Bullet = b;
        m_Timer = Wait();
        StartCoroutine(m_Timer);
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(m_Time);

        m_Bullet.m_BulletCallbacks.OnBulletDestroyed(m_Bullet);

        if (m_Bullet != null)
            m_Bullet.Kill();
    }

}

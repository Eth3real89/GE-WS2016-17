using System;
using System.Collections;
using UnityEngine;

public class WaveConeBulletAttack : BulletAttack
{
    private GameObject m_BulletPrefab;

    private bool m_Cancelled;

    public WaveConeBulletAttack(MonoBehaviour monoBehavior, GameObject bulletPrefab) : base(monoBehavior)
    {
        m_BulletPrefab = bulletPrefab;
        m_Cancelled = false;
    }

    public override void StartAttack()
    {
        for (int i = 0; i < 5; i++)
        {
            m_MonoBehaviour.StartCoroutine(LaunchWave(1 + 0.5f * i));
        }

        m_MonoBehaviour.StartCoroutine(EndAttackAfter(7f));
    }

    private IEnumerator LaunchWave(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);

        if (!m_Cancelled)
            CreateWaveBullets();
    }

    private IEnumerator EndAttackAfter(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);

        if(!m_Cancelled)
        {
            m_MonoBehaviour.StartCoroutine(DestroyAllBulletsAfter(3f));
            m_Callbacks.OnAttackEnd(this);
        }
    }

    public override void CancelAttack()
    {
        DestroyAllBullets();
        base.CancelAttack();
        m_Cancelled = true;
    }

    private IEnumerator DestroyAllBulletsAfter(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyAllBullets();
    }

    private void DestroyAllBullets()
    {
        for (int i = 0; i < m_Bullets.Count; i++)
        {
            m_Bullets[i].DestroyBullet();
        }
        m_Bullets.Clear();
    }

    private void CreateWaveBullets()
    {
        for(int i = 0; i < 5; i++)
        {
            CreateSingleBullet(i);
        }
    }

    private void CreateSingleBullet(int index)
    {
        GameObject boss = GameController.Instance.m_Boss;

        GameObject bullet = GameObject.Instantiate<GameObject>(m_BulletPrefab,
            boss.transform.position,
            boss.transform.rotation);

        StraightBullet sbScript = bullet.GetComponentInChildren<StraightBullet>();
        if (sbScript != null)
        {
            sbScript.m_Angle = boss.transform.rotation.eulerAngles.y + 20 * (index - 2f);
            sbScript.m_MaxLifetime = 5f;
            sbScript.m_Velocity = 2f;
        }

        m_Bullets.Add(sbScript);
    }
}

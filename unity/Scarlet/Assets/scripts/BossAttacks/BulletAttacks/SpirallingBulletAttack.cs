using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpirallingBulletAttack : BulletAttack
{
    private GameObject m_BulletPrefab;

    private bool m_Cancelled;
    private float m_InitialRotation;

    public SpirallingBulletAttack(MonoBehaviour monoBehavior, GameObject bulletPrefab) : base(monoBehavior)
    {
        m_BulletPrefab = bulletPrefab;
        m_Cancelled = false;
        m_InitialRotation = GameController.Instance.m_Boss.transform.rotation.eulerAngles.y;
    }

    public override void StartAttack()
    {
        for (int i = 0; i < 10; i++)
        {
            m_MonoBehaviour.StartCoroutine(LaunchWave(1 + 0.5f * i, i >= 5));
        }

        m_MonoBehaviour.StartCoroutine(EndAttackAfter(10f));
    }

    private IEnumerator LaunchWave(float timeDelay, bool clockwise)
    {
        yield return new WaitForSeconds(timeDelay);

        if (!m_Cancelled)
            CreateWaveBullets(clockwise);
    }

    private IEnumerator EndAttackAfter(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);

        if (!m_Cancelled)
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

    private void CreateWaveBullets(bool clockwise)
    {
        for (int i = 0; i < 6; i++)
        {
            CreateSingleBullet(i, clockwise);
        }
    }

    private void CreateSingleBullet(int index, bool clockwise)
    {
        GameObject boss = GameController.Instance.m_Boss;

        GameObject bullet = GameObject.Instantiate<GameObject>(m_BulletPrefab,
            boss.transform.position,
            Quaternion.Euler(0, m_InitialRotation + 60 * index, 0)
            );

        CurveBullet cbScript = bullet.GetComponentInChildren<CurveBullet>();
        if (cbScript != null)
        {
            cbScript.m_MaxLifetime = 8f;
            cbScript.m_Velocity = 2f;
            cbScript.m_Clockwise = clockwise;
        }

        m_Bullets.Add(cbScript);
    }
}

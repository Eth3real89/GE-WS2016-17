using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWaveAttack : GrowingAEAttack {

    public float m_Damage;

    public Transform m_Center;
    public float m_InitialFrontSize;
    public float m_DistanceBetweenCircles;
    public float m_GrowTime;
    public float m_GrowRate;

    public float m_Angles;
    private float m_AngleAtLaunch;

    public BlastWaveVisuals m_Visuals;

    public PlayerHittable m_Target;

    private IEnumerator m_GrowEnumerator;
    private bool m_HasHit;

    private BlastWaveDamage m_BlastDamage;

    public override void StartAttack()
    {
        base.StartAttack();
        

        m_BlastDamage = new BlastWaveDamage();
        m_BlastDamage.m_DamageAmount = this.m_Damage;

        m_HasHit = false;

        m_GrowEnumerator = GrowWave();
        StartCoroutine(m_GrowEnumerator);
    }

    private IEnumerator GrowWave()
    {
        m_AngleAtLaunch = m_Boss.transform.rotation.eulerAngles.y;
        if (m_AngleAtLaunch > 180) m_AngleAtLaunch -= 360;
        else if (m_AngleAtLaunch < -180) m_AngleAtLaunch += 360;

        m_Visuals.m_Angles = m_Angles;
        m_Visuals.Setup();

        yield return new WaitForEndOfFrame();
        m_Visuals.gameObject.SetActive(true);

        float waveSize = m_InitialFrontSize;
        m_Visuals.m_LineWidthFactor = m_DistanceBetweenCircles;

        float t = 0;
        while((t += Time.deltaTime) < m_GrowTime)
        {
            waveSize += m_GrowRate * Time.deltaTime;
            float distance = Vector3.Distance(m_Center.position, m_Target.transform.position);

            if (WithinDistanceBounds(waveSize, distance) && WithinAngleBounds(m_Angles))
            {
                DealDamage();
            }

            m_Visuals.ScaleUp(waveSize);

            yield return null;
        }

        m_Callback.OnAttackEnd(this);
        HideLightGuard();
        m_Visuals.gameObject.SetActive(false);
    }

    private bool WithinAngleBounds(float angles)
    {
        float angle = BossTurnCommand.CalculateAngleTowards(m_Boss.transform.position, m_Target.transform.position);
        
        if (m_AngleAtLaunch < 0)
        {
            while (angle > 0)
            {
                angle -= 360;
            }
        }
        else
        {
            while (angle < 0)
            {
                angle += 360;
            }
        }

        if (angle <= m_AngleAtLaunch + m_Angles / 2 && angle >= m_AngleAtLaunch - m_Angles / 2)
            return true;

        return false;
    }

    private bool WithinDistanceBounds(float waveSize, float distance)
    {
        return waveSize >= distance && waveSize - m_DistanceBetweenCircles / 4 <= distance;
    }

    public override void CancelAttack()
    {
        base.CancelAttack();

        if (m_GrowEnumerator != null)
            StopCoroutine(m_GrowEnumerator);
        
        m_Visuals.gameObject.SetActive(false);
    }

    private void DealDamage()
    {
        if (m_HasHit)
            return;

        m_Target.Hit(m_BlastDamage);
        m_HasHit = true;
    }

    private class BlastWaveDamage : AEAttackDamage
    {

    }
}

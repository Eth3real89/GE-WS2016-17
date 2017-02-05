using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWaveAttack : GrowingAEAttack {

    public static string START_EVENT_NAME = "blast_wave_start";

    public float m_Damage;

    public Transform m_Center;
    public float m_InitialFrontSize;
    public float m_DistanceBetweenCircles;
    public float m_GrowTime;
    public float m_GrowRate;

    public float m_WaveSize;

    public float m_Angles;
    private float m_AngleAtLaunch;

    public BlastWaveVisuals m_Visuals;

    public PlayerHittable m_Target;
    private Vector3 m_InitialCenterPos;

    private IEnumerator m_GrowEnumerator;
    private bool m_HasHit;

    public AEAttackDamage m_BlastDamage;

    public override void StartAttack()
    {
        base.StartAttack();

        EventManager.TriggerEvent(START_EVENT_NAME);

        m_Visuals.transform.position = new Vector3(m_Center.transform.position.x, m_Visuals.transform.position.y, m_Center.transform.position.z);
        m_InitialCenterPos = new Vector3(m_Center.transform.position.x, m_Center.transform.position.y, m_Center.transform.position.z);

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

        m_WaveSize = m_InitialFrontSize;
        m_Visuals.m_LineWidthFactor = m_DistanceBetweenCircles;

        float t = 0;
        while((t += Time.deltaTime) < m_GrowTime)
        {
            m_WaveSize += m_GrowRate * Time.deltaTime;
            float distance = Vector3.Distance(m_Target.transform.position, m_InitialCenterPos);

            if (WithinDistanceBounds(m_WaveSize, distance) && WithinAngleBounds(m_Angles))
            {
                DealDamage();
            }

            m_Visuals.ScaleUp(m_WaveSize);

            yield return null;
        }

        m_Callback.OnAttackEnd(this);
        HideLightGuard();
        m_Visuals.gameObject.SetActive(false);
    }

    private bool WithinAngleBounds(float angles)
    {
        float angle = BossTurnCommand.CalculateAngleTowards(m_Target.transform.position, m_InitialCenterPos);
        
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
}

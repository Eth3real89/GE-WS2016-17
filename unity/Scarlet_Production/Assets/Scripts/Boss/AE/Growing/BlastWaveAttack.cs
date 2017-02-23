using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWaveAttack : GrowingAEAttack {

    public static string START_EVENT_NAME = "blast_wave_start";
    public static string ATTACK_HIT_EVENT = "blast_wave_hit";

    public float m_Damage;

    public Transform m_Center;
    public float m_InitialFrontSize;
    public float m_DistanceBetweenCirclesStart;
    public float m_DistanceBetweenCirclesEnd = -1;
    protected float m_DistanceBetweenCircles;

    public float m_GrowTime;
    public float m_GrowRate;

    public float m_WaveSize;

    public float m_Angles;
    protected float m_AngleAtLaunch;

    public BlastWaveVisuals m_Visuals;

    public PlayerHittable m_Target;
    protected Vector3 m_InitialCenterPos;

    private IEnumerator m_GrowEnumerator;
    protected bool m_HasHit;

    public AEAttackDamage m_BlastDamage;

    public override void StartAttack()
    {
        base.StartAttack();

        m_Visuals.transform.parent = null;
        m_Visuals.gameObject.SetActive(true);

        EventManager.TriggerEvent(START_EVENT_NAME);

        m_Visuals.transform.position = new Vector3(m_Center.transform.position.x, m_Visuals.transform.position.y, m_Center.transform.position.z);
        m_InitialCenterPos = new Vector3(m_Center.transform.position.x, m_Center.transform.position.y, m_Center.transform.position.z);

        m_BlastDamage.m_DamageAmount = this.m_Damage;

        if (m_DistanceBetweenCirclesEnd < 0)
            m_DistanceBetweenCirclesEnd = m_DistanceBetweenCirclesStart;

        m_HasHit = false;

        m_GrowEnumerator = GrowWave();
        StartCoroutine(m_GrowEnumerator);
    }

    protected virtual IEnumerator GrowWave()
    {
        yield return GrowRoutine();

        m_Callback.OnAttackEnd(this);
        m_Visuals.gameObject.SetActive(false);
    }

    protected virtual IEnumerator GrowRoutine()
    {
        m_AngleAtLaunch = m_Boss.transform.rotation.eulerAngles.y;
        if (m_AngleAtLaunch > 180) m_AngleAtLaunch -= 360;
        else if (m_AngleAtLaunch < -180) m_AngleAtLaunch += 360;

        m_Visuals.m_Angles = m_Angles;
        m_Visuals.Setup();

        yield return new WaitForEndOfFrame();
        m_Visuals.gameObject.SetActive(true);

        m_WaveSize = m_InitialFrontSize;
        m_Visuals.m_LineWidthFactor = m_DistanceBetweenCirclesStart;

        float t = 0;
        while ((t += Time.deltaTime) < m_GrowTime)
        {
            m_WaveSize += m_GrowRate * Time.deltaTime;
            float distance = Vector3.Distance(m_Target.transform.position - new Vector3(0, m_Target.transform.position.y, 0), m_InitialCenterPos - new Vector3(0, m_InitialCenterPos.y, 0));

            m_DistanceBetweenCircles = m_DistanceBetweenCirclesStart + (t / m_GrowTime) * (m_DistanceBetweenCirclesEnd - m_DistanceBetweenCirclesStart);

            if (WithinDistanceBounds(m_WaveSize, distance, m_DistanceBetweenCircles) && WithinAngleBounds(m_Angles))
            {
                DealDamage();
            }
            
            m_Visuals.m_LineWidthFactor = m_DistanceBetweenCircles;
            m_Visuals.ScaleUp(m_WaveSize);

            yield return null;
        }
    }

    protected virtual bool WithinAngleBounds(float angles)
    {
        float angle = BossTurnCommand.CalculateAngleTowards(m_InitialCenterPos, m_Target.transform.position);
        
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

    protected virtual bool WithinDistanceBounds(float waveSize, float distance, float distanceBetweenCircles)
    {
        return waveSize >= distance && waveSize - distanceBetweenCircles <= distance;
    }

    public override void CancelAttack()
    {
        base.CancelAttack();

        if (m_GrowEnumerator != null)
            StopCoroutine(m_GrowEnumerator);
        
        m_Visuals.gameObject.SetActive(false);
    }

    protected virtual void DealDamage()
    {
        if (m_HasHit)
            return;

        EventManager.TriggerEvent(ATTACK_HIT_EVENT);
        m_Target.Hit(m_BlastDamage);
        m_HasHit = true;
    }
}

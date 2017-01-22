using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWaveAttack : GrowingAEAttack, GrowingAEFrontWave.FrontWaveCallback, GrowingAEBackWave.BackWaveCallback {

    public float m_Damage;

    public Transform m_Center;
    public float m_InitialFrontSize;
    public float m_DistanceBetweenCircles;
    public float m_GrowTime;
    public float m_GrowRate;

    public BlastWaveVisuals m_Visuals;

    public PlayerHittable m_Target;

    private IEnumerator m_GrowEnumerator;
  
    private bool m_ScarletInFront;
    private bool m_ScarletInBack;
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
        m_Visuals.gameObject.SetActive(true);
        m_Visuals.Setup();

        float waveSize = m_InitialFrontSize;
        m_Visuals.m_LineWidthFactor = m_DistanceBetweenCircles;

        float t = 0;
        while((t += Time.deltaTime) < m_GrowTime)
        {
            waveSize += m_GrowRate * Time.deltaTime;
            float distance = Vector3.Distance(m_Center.position, m_Target.transform.position);
            
            if (waveSize >= distance && waveSize - m_DistanceBetweenCircles / 2 <= distance)
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
    

    public void NotifyAboutRangeFront(bool isInFront)
    {
        m_ScarletInFront = isInFront;
        if (isInFront)
            StartCoroutine(CheckHit());
    }

    private IEnumerator CheckHit()
    {
        yield return new WaitForEndOfFrame();
        if (!m_ScarletInBack && m_ScarletInFront)
            DealDamage();
    }

    public void NotifyAboutRangeBack(bool isInBack)
    {
        m_ScarletInBack = isInBack;
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

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
    public GrowingAEFrontWave m_FrontWave;
    public GrowingAEBackWave m_BackWave;

    private IEnumerator m_GrowEnumerator;
    private bool m_ScarletInFront;
    private bool m_ScarletInBack;
    private bool m_HitInNextUpdate;

    public override void StartAttack()
    {
        base.StartAttack();

        m_FrontWave.m_Callback = this;
        m_BackWave.m_Callback = this;

        m_Visuals.gameObject.SetActive(true);
        m_Visuals.m_LineWidthFactor = m_DistanceBetweenCircles;
        m_Visuals.Setup();
        m_FrontWave.gameObject.SetActive(true);
        m_BackWave.gameObject.SetActive(true);

        m_GrowEnumerator = GrowWave();
        StartCoroutine(m_GrowEnumerator);
    }

    private IEnumerator GrowWave()
    {
        m_FrontWave.transform.localScale = m_FrontWave.transform.localScale.normalized * m_InitialFrontSize;
        m_BackWave.transform.localScale = m_BackWave.transform.localScale.normalized * ((m_InitialFrontSize - m_DistanceBetweenCircles > 0)? m_InitialFrontSize - m_DistanceBetweenCircles : 0);

        bool xSmaller = m_FrontWave.transform.localScale.x < m_FrontWave.transform.localScale.z;

        float t = 0;
        while((t += Time.deltaTime) < m_GrowTime)
        {
            HandleGrowth(xSmaller);

            if (m_HitInNextUpdate)
            {
                if (m_ScarletInFront && !m_ScarletInBack)
                    MLog.Log(LogType.AELog, 0, "Blast Wave Attack: Hit!!");

                m_HitInNextUpdate = false;
            }

            yield return null;
        }

        m_Callback.OnAttackEnd(this);
        m_FrontWave.gameObject.SetActive(false);
        m_BackWave.gameObject.SetActive(false);
        m_Visuals.gameObject.SetActive(false);
    }

    private void HandleGrowth(bool xSmaller)
    {
        Vector3 growth = new Vector3(m_GrowRate, m_GrowRate, m_GrowRate) * Time.deltaTime;

        m_FrontWave.transform.localScale += growth * 2;

        float compareValue = xSmaller? m_FrontWave.transform.localScale.x : m_FrontWave.transform.localScale.z;
        if (compareValue >= m_DistanceBetweenCircles)
        {
            m_BackWave.transform.localScale = m_FrontWave.transform.localScale - new Vector3(m_DistanceBetweenCircles, 0, m_DistanceBetweenCircles);
        }


        m_Visuals.ScaleUp((m_FrontWave.transform.localScale - new Vector3(m_DistanceBetweenCircles, 0, m_DistanceBetweenCircles) / 2) / 2);
    }

    public void NotifyAboutRangeFront(bool isInFront)
    {
        m_ScarletInFront = isInFront;
        if (isInFront)
            m_HitInNextUpdate = true;
    }

    public void NotifyAboutRangeBack(bool isInBack)
    {
        m_ScarletInBack = isInBack;
    }
}

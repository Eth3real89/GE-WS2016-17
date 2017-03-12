using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPathIndicator : BossAttack
{
    public GameObject m_StartPoint;
    public GameObject m_EndPoint;

    public GameObject m_Indicator;

    public Color m_From;
    public Color m_To;

    public float[] m_Times;

    protected int m_Index;
    protected IEnumerator m_Timer;

    public float m_ScaleAdjustment = 10f;

    public override void StartAttack()
    {
        base.StartAttack();

        m_Indicator.SetActive(true);
        m_Index = 0;

        m_Timer = LightUp(m_Times[m_Index]);
        StartCoroutine(m_Timer);
    }

    protected virtual IEnumerator LightUp(float time)
    {
        Renderer r = m_Indicator.GetComponentInChildren<Renderer>();

        float t = 0;
        while((t += Time.deltaTime) < time)
        {
            float factor = (t / time) * (t / time);

            m_EndPoint.GetComponentInChildren<ShowTargetRing>().UpdateIndicator(t / time);
            r.material.color = Color.Lerp(m_From, m_To, factor);

            r.material.SetColor(Shader.PropertyToID("_EmissionColor"), Color.Lerp(m_From, m_To, factor));

            m_Indicator.transform.localScale = new Vector3(m_Indicator.transform.localScale.x,
                m_Indicator.transform.localScale.y, Vector3.Distance(m_StartPoint.transform.position, m_EndPoint.transform.position) * m_ScaleAdjustment);

            m_Indicator.transform.position = m_StartPoint.transform.position;
            m_Indicator.transform.rotation = Quaternion.Euler(0, BossTurnCommand.CalculateAngleTowards(m_StartPoint.transform.position, m_EndPoint.transform.position), 0);

            DynamicGI.UpdateMaterials(r);

            yield return null;
        }

        m_Index++;

        if (m_Index < m_Times.Length)
        {
            m_Timer = LightUp(m_Times[m_Index]);
            StartCoroutine(m_Timer);
        }
        else
        {
            m_Indicator.SetActive(false);
        }
    }

    public override void CancelAttack()
    {
        if (m_Timer != null)
            StopCoroutine(m_Timer);

        m_Indicator.SetActive(false);
    }
}

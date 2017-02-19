using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitWarningMaterialBased : HitWarning {

    public float m_WarningBuildUpTime = 0.2f;
    public float m_MaxValue = 0.8f;

    public Renderer[] m_AffectedRenderers;
    public GameObject[] m_Objects;
    public Material[] m_Materials;

    protected IEnumerator m_HighlightEnumerator;

    public override void HideWarning(int attackAnimation)
    {
        if (m_HighlightEnumerator != null)
            StopCoroutine(m_HighlightEnumerator);
        SetEmissiveColor(0, 0, 0, 0);
        UpdateRenderers();
        SetObjectsActive(false);
    }

    private void SetObjectsActive(bool v)
    {
        foreach (GameObject go in m_Objects)
        {
            go.SetActive(v);
        }
    }

    public override void ShowWarning(int attackAnimation)
    {
        SetObjectsActive(true);

        m_HighlightEnumerator = HighlightTimer();
        StartCoroutine(m_HighlightEnumerator);
    }

    protected IEnumerator HighlightTimer()
    {
        float t = 0;
        while((t += Time.deltaTime) < m_WarningBuildUpTime)
        {
            float col = t / m_WarningBuildUpTime * m_MaxValue;
            SetEmissiveColor(col, col, col, col);
            UpdateRenderers();
            yield return null;
        }
    }

    protected void SetEmissiveColor(float r, float g, float b, float a)
    {
        foreach (Material m in m_Materials)
        {
            m.SetColor("_EmissionColor", new Color(r, g, b, a));
        }
    }

    protected void UpdateRenderers()
    {
        foreach(Renderer r in m_AffectedRenderers)
        {
            DynamicGI.UpdateMaterials(r);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConeCanvasBasedVisuals : MonoBehaviour, GrowingConeAttackVisuals
{
    // trial and error based on the specific png image & based on a canvas with scale 1x1
    private const float m_ScaleFactor = 1.9f;

    public Canvas m_Canvas;
    public Slider m_Slider;

    public float m_Size;
    public float m_Angle;

    public void ShowAttack()
    {
        this.m_Canvas.gameObject.SetActive(true);
        this.m_Canvas.transform.localScale = new Vector3(m_Size, m_Size, m_Size);
        this.m_Slider.value = m_Angle;

        this.m_Canvas.transform.rotation = Quaternion.Euler(90, this.transform.eulerAngles.y - m_Angle / 2, 0);

        Material m = m_Canvas.GetComponentInChildren<Image>().material;
        m_Canvas.GetComponentInChildren<Image>().material = Instantiate(m);
    }

    public void HideAttack()
    {
        this.m_Canvas.gameObject.SetActive(false);
    }

    public void SetAngle(float angle)
    {
        m_Angle = angle;
        this.m_Slider.value = m_Angle;
    }

    public float GetAngle()
    {
        return m_Angle;
    }

    public void SetRadius(float radius)
    {
        this.m_Size = radius * m_ScaleFactor;
        this.m_Canvas.transform.localScale = new Vector3(m_Size, m_Size, m_Size);
    }

    public void ScaleTo(Vector3 scale)
    {
        this.m_Canvas.transform.localScale = scale * m_ScaleFactor;
    }

    public void UpdateVisuals()
    {
    }

    public void SetStartRadius(float radius)
    {
        m_Canvas.GetComponentInChildren<Image>().material.SetFloat(Shader.PropertyToID("_CutUpTo"), radius * 0.25f);
    }

    public void SetColor(Color c)
    {
        int colorId = Shader.PropertyToID("_Color");
        Material m = m_Canvas.GetComponentInChildren<Image>().material;
        m.SetColor(colorId, c);
    }
}

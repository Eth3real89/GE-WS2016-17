using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInRotatingConeAttack : GrowingThenRotatingConeAttack {

    public Material m_SeeThroughConeMaterial;

    public Color m_FlickerFromColorSetup = Color.black;
    public Color m_FlickerToColorSetup = Color.white;
    
    public int m_FlickerTimesSetup;

    public Color m_FlickerFromColorAttack = Color.gray;
    public Color m_FlickerToColorAttack = Color.white;

    public int m_FlickerTimesAttack;

    public bool m_FlickerOut = false;

    public float m_MinDistance = 0;

    /// <summary>
    /// how long flicker out lasts relative to flicker in.
    /// </summary>
    public float m_FadeInOutRatio = 4f;

    protected Color m_LastColor;

    protected override IEnumerator Grow()
    {
        m_AttackVisuals.SetAngle(m_Angle);
        m_Damage.m_Angle = m_Angle;

        m_Damage.m_Distance = m_EndSize;
        m_Damage.m_MinDistance = m_MinDistance;

        m_AttackVisuals.SetRadius(m_EndSize);
        m_AttackVisuals.ShowAttack();


        int colorId = Shader.PropertyToID("_Color");
        Material m = m_AttackVisualsContainer.GetComponentInChildren<Image>().material;
        m_AttackVisualsContainer.GetComponentInChildren<Image>().material = Instantiate(m);

        m_AttackVisualsContainer.GetComponentInChildren<Image>().material.SetFloat(Shader.PropertyToID("_CutUpTo"), m_MinDistance / m_EndSize * .25f);

        float t = 0;
        while ((t += Time.deltaTime) < m_GrowTime)
        {

            m = m_AttackVisualsContainer.GetComponentInChildren<Image>().material;
            float colorDeterminer = 1 - Mathf.Abs(Mathf.Cos(t / m_GrowTime * (m_FlickerTimesSetup + 0.5f) * 180 * Mathf.Deg2Rad));
            m.SetColor(colorId, Color.Lerp(m_FlickerFromColorSetup, m_FlickerToColorSetup, colorDeterminer));

            yield return null;
        }

        m_Damage.Activate();
        AfterGrow();
    }

    protected override void AfterGrow()
    {
        m_GrowEnumerator = Rotate();
        StartCoroutine(m_GrowEnumerator);
    }

    protected override IEnumerator Rotate()
    {
        int colorId = Shader.PropertyToID("_Color");

        float t = 0;
        while ((t += Time.deltaTime) < m_RotationTime)
        {
            m_Boss.transform.Rotate(Vector3.up, m_RotationAngle * Time.deltaTime / m_RotationTime);

            Material m = m_AttackVisualsContainer.GetComponentInChildren<Image>().material;
            float colorDeterminer = 1 - Mathf.Abs(Mathf.Cos(t / m_RotationTime * (m_FlickerTimesAttack + 0.5f) * 180 * Mathf.Deg2Rad));

            m_LastColor = Color.Lerp(m_FlickerFromColorAttack, m_FlickerToColorAttack, colorDeterminer);
            m.SetColor(colorId, m_LastColor);

            yield return null;
        }

        AfterRotate();
    }

    protected virtual void AfterRotate()
    {

        if (m_LeaveCone)
        {
            m_Callback.OnAttackEnd(this);
            return;
        }

        m_Damage.DisableDamage();
        m_Callback.OnAttackEnd(this);

        if (m_FlickerOut)
        {
            StartCoroutine(FlickerOut());
        }
        else
        {
            m_AttackVisuals.HideAttack();
        }
    }

    private IEnumerator FlickerOut()
    {
        int colorId = Shader.PropertyToID("_Color");

        float t = 0;
        while ((t += Time.deltaTime) < m_GrowTime / m_FadeInOutRatio)
        {
            Material m = m_AttackVisualsContainer.GetComponentInChildren<Image>().material;
            m.SetColor(colorId, Color.Lerp(m_FlickerToColorSetup, Color.black, t / (m_GrowTime / m_FadeInOutRatio)));

            yield return null;
        }
        m_AttackVisuals.HideAttack();
    }

}

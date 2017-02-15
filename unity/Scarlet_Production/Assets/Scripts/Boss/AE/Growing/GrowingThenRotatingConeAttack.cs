using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingThenRotatingConeAttack : GrowingConeAttack
{

    public float m_RotationTime = 15f;
    public float m_RotationAngle = 360;

    public bool m_TakeOverCone;

    protected override IEnumerator Grow()
    {
        if (m_TakeOverCone && m_AttackVisuals.m_Angle != 0)
        {
            yield return AdjustCone();
        }
        else
        {
            yield return base.Grow();
        }
    }

    private IEnumerator AdjustCone()
    {
        float initialAngle = m_AttackVisuals.m_Angle;

        if (initialAngle == 0 && m_Angle != 0)
        {
            initialAngle = 1f;
            m_AttackVisuals.m_Angle = initialAngle;

            m_AttackVisuals.transform.localScale = new Vector3(1, 0, 1) * m_EndSize;
            m_Damage.m_Distance = m_EndSize;

            m_AttackVisuals.ShowAttack();
            yield return null;

            NormalizeLines();
        }

        float t = 0;
        while((t += Time.deltaTime) < m_GrowTime)
        {
            if (initialAngle != 0)
            {
                float angle = Mathf.Lerp(initialAngle, m_Angle, t / m_GrowTime);
                m_AttackVisuals.m_Angle = angle;
                m_AttackVisuals.UpdateLines();
                m_Damage.m_Angle = m_Angle;
            }

            yield return null;
        }

        if (m_Angle == 0)
        {
            m_AttackVisuals.HideAttack();
        }

        m_AttackVisuals.m_Angle = m_Angle;
        m_Damage.m_Angle = m_Angle;

        AfterGrow();
    }

    private void NormalizeLines()
    {
        GameObject[] lines = m_AttackVisuals.GetLines();
        m_LineMaterials = new Material[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            m_LineMaterials[i] = lines[i].GetComponent<MeshRenderer>().material;
        }
        int id = Shader.PropertyToID("_LineWidth");
        int id2 = Shader.PropertyToID("_LineScale");

        for (int i = 0; i < m_LineMaterials.Length; i++)
        {
            m_LineMaterials[i].SetFloat(id, 1 / m_LineMaterials[i].GetFloat(id2));
        }
    }

    protected override void AfterGrow()
    {
        m_GrowEnumerator = Rotate();
        StartCoroutine(m_GrowEnumerator);
    }

    protected virtual IEnumerator Rotate()
    {
        float t = 0;
        while((t += Time.deltaTime) < m_RotationTime)
        {
            m_Boss.transform.Rotate(Vector3.up, m_RotationAngle * Time.deltaTime / m_RotationTime);
            yield return null;
        }

        base.AfterGrow();
    }

}

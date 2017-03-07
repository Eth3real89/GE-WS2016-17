using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularAttackVisuals : FixedPlaceAttackVisuals, GrowingConeAttackVisuals {

    public GameObject m_VolumetricLinePrefab;

    protected GameObject[] m_Lines;
    protected Material[] m_LineMaterials;

    public float m_Size;
    public float m_Angle;

    public bool m_SetFixedNumLines = false;
    public int m_FixedNumLines = 15;

    private int m_NumPoints;

    protected int m_LineWidthId;
    protected int m_LineScaleId;

    public override void ShowAttack()
    {
        base.ShowAttack();

        m_LineWidthId = Shader.PropertyToID("_LineWidth");
        m_LineScaleId = Shader.PropertyToID("_LineScale");

        if (m_Angle == 0)
        {
            m_Lines = new GameObject[0];
            return;
        }
            
        CreateLines();
    }

    public override void HideAttack()
    {
        base.HideAttack();

        if (m_Lines == null)
            return;

        foreach(GameObject go in m_Lines)
        {
            if (go == null)
                continue;
            GameObject.Destroy(go);
        }
    }

    private void CreateLines()
    {
        if (m_VolumetricLinePrefab == null)
            return;

        m_NumPoints = Mathf.Max(15, (int) (m_Angle / 10));

        int numLines = (m_SetFixedNumLines)? m_FixedNumLines : (int) Mathf.Max(m_Size, 1) * 2;
        m_Lines = new GameObject[numLines - 1];

        for(int i = 1; i < numLines; i++)
        {
            m_Lines[i - 1] = CreateSingleLine(i, numLines);
        }
    }

    public void UpdateLines()
    {
        if (m_VolumetricLinePrefab == null)
            return;

        m_NumPoints = Mathf.Max(15, (int)(m_Angle / 10));

        for (int i = 1; i < m_Lines.Length; i++)
        {
            UpdateSingleLine(i, m_Lines.Length);
        }
    }

    public GameObject[] GetLines()
    {
        return m_Lines;
    }

    private GameObject CreateSingleLine(int index, int numLines)
    {
        GameObject line = GameObject.Instantiate(m_VolumetricLinePrefab, transform.position, transform.rotation);
        line.transform.parent = this.transform;
        line.transform.position = line.transform.position + new Vector3(0, 0.25f, 0);
        line.transform.localScale = new Vector3(1, 1, 1);

        float angleStep = m_Angle / m_NumPoints;

        Vector3[] vertices = new Vector3[m_NumPoints];

        for(int i = 0; i < m_NumPoints; i++)
        {
            vertices[i] = CalculateLinePoint(index, i, angleStep, numLines);
        }

        VolumetricLines.VolumetricLineStripBehavior behavior = line.GetComponent<VolumetricLines.VolumetricLineStripBehavior>();
        if (behavior == null)
            return null;
        behavior.UpdateLineVertices(vertices);

        return line;
    }

    private void UpdateSingleLine(int index, int numLines)
    {

        float angleStep = m_Angle / m_NumPoints;
        Vector3[] vertices = new Vector3[m_NumPoints];

        for (int i = 0; i < m_NumPoints; i++)
        {
            vertices[i] = CalculateLinePoint(index, i, angleStep, numLines);
        }

        VolumetricLines.VolumetricLineStripBehavior behavior = m_Lines[index].GetComponent<VolumetricLines.VolumetricLineStripBehavior>();
        behavior.UpdateLineVertices(vertices);
    }

    
    private Vector3 CalculateLinePoint(int lineIndex, int pointIndex, float angleStep, int numLines)
    {
        float distance = lineIndex / (float) numLines * m_Size / 2;

        int angleMultiplier = (- m_NumPoints / 2) + pointIndex;

        Vector3 relativePos = Quaternion.Euler(0, angleMultiplier * angleStep, 0) * new Vector3(0, 0, distance);

        return relativePos;
    }

    public void SetAngle(float angle)
    {
        m_Angle = angle;
    }

    public void SetRadius(float radius)
    {
        m_Size = radius;
    }

    public void ScaleTo(Vector3 scale)
    {
        transform.localScale = new Vector3(scale.x, 0, scale.z);

        m_LineMaterials = new Material[m_Lines.Length];
        for (int i = 0; i < m_Lines.Length; i++)
        {
            m_LineMaterials[i] = m_Lines[i].GetComponent<MeshRenderer>().material;
        }
        for (int i = 0; i < m_LineMaterials.Length; i++)
        {
            m_LineMaterials[i].SetFloat(m_LineWidthId, 1f / m_LineMaterials[i].GetFloat(m_LineScaleId) );
        }
    }

    public float GetAngle()
    {
        return m_Angle;
    }

    public void UpdateVisuals()
    {
        UpdateLines();
    }

    public void SetStartRadius(float radius)
    {
        // not implemented, won't need to be
        // (may possibly need to be, if we use this attack again ~.~ )
    }

    public void SetColor(Color c)
    {
        // not implemented, won't need to be
    }
}

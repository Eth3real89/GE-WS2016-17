using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularAttackVisuals : FixedPlaceAttackVisuals {

    public GameObject m_VolumetricLinePrefab;

    private GameObject[] m_Lines;

    public float m_Size;
    public float m_Angle;

    public bool m_SetFixedNumLines = false;
    public int m_FixedNumLines = 15;

    private int m_NumPoints;

    public override void ShowAttack()
    {
        base.ShowAttack();

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

        VolumetricLines.VolumetricLineStripBehavior behavior = line.GetComponent<VolumetricLines.VolumetricLineStripBehavior>();
        if (behavior == null)
            return null;

        float angleStep = m_Angle / m_NumPoints;

        Vector3[] vertices = new Vector3[m_NumPoints];

        for(int i = 0; i < m_NumPoints; i++)
        {
            vertices[i] = CalculateLinePoint(index, i, angleStep, numLines);
        }

        behavior.UpdateLineVertices(vertices);

        return line;
    }

    private void UpdateSingleLine(int index, int numLines)
    {
        VolumetricLines.VolumetricLineStripBehavior behavior = m_Lines[index].GetComponent<VolumetricLines.VolumetricLineStripBehavior>();

        float angleStep = m_Angle / m_NumPoints;
        Vector3[] vertices = new Vector3[m_NumPoints];

        for (int i = 0; i < m_NumPoints; i++)
        {
            vertices[i] = CalculateLinePoint(index, i, angleStep, numLines);
        }

        behavior.UpdateLineVertices(vertices);
    }

    
    private Vector3 CalculateLinePoint(int lineIndex, int pointIndex, float angleStep, int numLines)
    {
        float distance = lineIndex / (float) numLines * m_Size / 2;

        int angleMultiplier = (- m_NumPoints / 2) + pointIndex;

        Vector3 relativePos = Quaternion.Euler(0, angleMultiplier * angleStep, 0) * new Vector3(0, 0, distance);

        return relativePos;
    }
}

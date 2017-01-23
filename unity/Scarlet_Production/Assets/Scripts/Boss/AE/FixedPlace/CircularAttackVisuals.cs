using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularAttackVisuals : FixedPlaceAttackVisuals {

    public GameObject m_VolumetricLinePrefab;

    private GameObject[] m_Lines;

    public float m_Size;
    public float m_Angle;

    private int m_NumPoints = 11;

    public override void ShowAttack()
    {
        base.ShowAttack();

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

        int numLines = (int) Mathf.Max(m_Size, 1);
        m_Lines = new GameObject[numLines - 1];

        for(int i = 1; i < numLines; i++)
        {
            m_Lines[i - 1] = CreateSingleLine(i, numLines);
        }
    }

    private GameObject CreateSingleLine(int index, int numLines)
    {
        GameObject line = GameObject.Instantiate(m_VolumetricLinePrefab, transform.position, transform.rotation);
        line.transform.position = line.transform.position + new Vector3(0, 0.25f, 0);

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

    private Vector3 CalculateLinePoint(int lineIndex, int pointIndex, float angleStep, int numLines)
    {
        float distance = lineIndex / (float) numLines * m_Size / 2;

        int angleMultiplier = (- m_NumPoints / 2) + pointIndex;

        Vector3 relativePos = Quaternion.Euler(0, angleMultiplier * angleStep, 0) * new Vector3(0, 0, distance);

        return relativePos;
    }
}

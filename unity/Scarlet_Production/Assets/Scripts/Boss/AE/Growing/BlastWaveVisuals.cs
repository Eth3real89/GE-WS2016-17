using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWaveVisuals : MonoBehaviour {

    public int m_NumVertices;
    public float m_InitialScale;

    public float m_Angles;

    public float m_LineWidthFactor = 1;
    public float m_AdjustForLightsaber = 1.4f;

    public Transform m_Center;

    public VolumetricLines.VolumetricLineStripBehavior m_volumetricBehavior;
    public Material m_VolumetricLineMaterial;

    public void Setup()
    {
        transform.localScale = transform.localScale.normalized * m_InitialScale;

        Vector3[] vertices;
        vertices = new Vector3[m_NumVertices + 1];
        float angle = m_Angles / m_NumVertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vert = new Vector3(0, m_Center.localPosition.y, 0) + Quaternion.Euler(0, angle * i - m_Angles / 2, 0) * new Vector3(0, 0, m_InitialScale / 2);
            vertices[i] = vert;
        }

        m_volumetricBehavior.UpdateLineVertices(vertices);
    }

    public void ScaleUp(float totalGrowth)
    {
        //        m_VolumetricLineMaterial.SetFloat(Shader.PropertyToID("_LineWidth"), 1 / m_VolumetricLineMaterial.GetFloat(Shader.PropertyToID("_LineScale")));

        transform.localScale = new Vector3(totalGrowth, 0.5f, totalGrowth) * 2;

        Material m = m_volumetricBehavior.GetComponent<MeshRenderer>().material;
        m.SetFloat(Shader.PropertyToID("_LineWidth"), m_AdjustForLightsaber * m_LineWidthFactor / totalGrowth);
    }
}

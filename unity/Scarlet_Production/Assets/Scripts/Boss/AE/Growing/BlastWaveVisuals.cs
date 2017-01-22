using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWaveVisuals : MonoBehaviour {

    public int m_NumVertices;
    public float m_InitialScale;

    public float m_LineWidthFactor = 2;

    public Transform m_Center;

    public VolumetricLines.VolumetricLineStripBehavior m_volumetricBehavior;
    public Material m_VolumetricLineMaterial;

    private float totalGrowth;

    public void Setup()
    {
        transform.localScale = transform.localScale.normalized * m_InitialScale;

        Vector3[] vertices = new Vector3[m_NumVertices + 1];
        float angle = 360f / m_NumVertices;

        for(int i = 0; i < vertices.Length; i++)
        {
            Vector3 vert = m_Center.localPosition + Quaternion.Euler(0, angle * i, 0) * new Vector3(0, 0, m_InitialScale);
            vertices[i] = vert;
        }

        m_volumetricBehavior.UpdateLineVertices(vertices);
        totalGrowth = 1;
    }

    public void ScaleUp(Vector3 totalGrowth)
    {
        transform.localScale = totalGrowth;
        //        m_VolumetricLineMaterial.SetFloat(Shader.PropertyToID("_LineWidth"), 1 / m_VolumetricLineMaterial.GetFloat(Shader.PropertyToID("_LineScale")));

        Material m = m_volumetricBehavior.GetComponent<MeshRenderer>().material;

        m.SetFloat(Shader.PropertyToID("_LineWidth"), m_LineWidthFactor / totalGrowth.magnitude);
    }
}

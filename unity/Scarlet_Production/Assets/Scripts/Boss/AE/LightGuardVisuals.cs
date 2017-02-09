using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGuardVisuals : MonoBehaviour {

    public int m_NumVertices;
    public float m_InitialScale;

    public float m_Angles;

    public int m_NumCircles = 4;

    public float m_LineWidthFactor = 1;
    public float m_AdjustForLightsaber = 1.4f;

    public Transform m_Center;

    public VolumetricLines.VolumetricLineStripBehavior m_volumetricBehavior1;
    public VolumetricLines.VolumetricLineStripBehavior m_volumetricBehavior2;

    private Transform m_PrevParent;

    public void Setup()
    {
        transform.localScale = transform.localScale.normalized * m_InitialScale;

        Vector3[] vertices;
        if (m_Angles == 360)
        {
            vertices = new Vector3[(m_NumVertices - 1) * m_NumCircles];
        }
        else
        {
            vertices = new Vector3[(m_NumVertices + 1) * m_NumCircles];
        }
        float angle = m_Angles / m_NumVertices;

        for(int i = 0; i < m_NumCircles; i++)
        {
            float distance = ((m_InitialScale / 2) / m_NumCircles) * (i + 1);

            float yDistance = Mathf.Cos(i * (90 / m_NumCircles) * Mathf.Deg2Rad) * m_InitialScale * 1.5f;

            for (int j = 0; j < vertices.Length / m_NumCircles; j++)
            {
                Vector3 vert = m_Center.localPosition + Quaternion.Euler(0, angle * j - m_Angles / 2, 0) * new Vector3(0, yDistance, distance);
                vertices[(i * vertices.Length / m_NumCircles) + j] = vert;
            }
        }

        m_volumetricBehavior1.UpdateLineVertices(vertices);
        m_volumetricBehavior2.UpdateLineVertices(vertices);
    }

    public void ScaleUp(float totalGrowth)
    {
        //        m_VolumetricLineMaterial.SetFloat(Shader.PropertyToID("_LineWidth"), 1 / m_VolumetricLineMaterial.GetFloat(Shader.PropertyToID("_LineScale")));

        transform.localScale = new Vector3(totalGrowth, 0.5f, totalGrowth) * 2;

        Material m = m_volumetricBehavior1.GetComponent<MeshRenderer>().material;
        m.SetFloat(Shader.PropertyToID("_LineWidth"), m_AdjustForLightsaber * m_LineWidthFactor / totalGrowth);
        m = m_volumetricBehavior2.GetComponent<MeshRenderer>().material;
        m.SetFloat(Shader.PropertyToID("_LineWidth"), m_AdjustForLightsaber * m_LineWidthFactor / totalGrowth);
    }

    public void DetachFromParent()
    {
        m_PrevParent = this.transform.parent;
        this.transform.parent = null;
    }

    public void ReattachToParent()
    {
        if (this.transform.parent != null)
            return;

        this.transform.parent = m_PrevParent;

        m_volumetricBehavior1.transform.rotation = Quaternion.Euler(0, 0, 0);
        m_volumetricBehavior2.transform.rotation = Quaternion.Euler(0, 180, 0);

        m_volumetricBehavior1.transform.position = new Vector3(m_PrevParent.transform.position.x, m_volumetricBehavior1.transform.position.y, m_PrevParent.transform.position.z);
        m_volumetricBehavior2.transform.position = new Vector3(m_PrevParent.transform.position.x, m_volumetricBehavior2.transform.position.y, m_PrevParent.transform.position.z);
    }
}

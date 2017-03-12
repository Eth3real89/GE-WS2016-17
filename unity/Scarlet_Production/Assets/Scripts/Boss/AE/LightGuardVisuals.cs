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

    private Transform m_PrevParent;

    public void Setup()
    {
        transform.localScale = transform.localScale.normalized * m_InitialScale;
    }

    public void ScaleUp(float totalGrowth)
    {
        //        m_VolumetricLineMaterial.SetFloat(Shader.PropertyToID("_LineWidth"), 1 / m_VolumetricLineMaterial.GetFloat(Shader.PropertyToID("_LineScale")));

        transform.localScale = new Vector3(1, 1, 1) * totalGrowth * 2;
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
    }
}

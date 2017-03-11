using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGuard : MonoBehaviour {

    public LightGuardVisuals m_LightGuardVisuals;

    public GameObject m_Center;

    public float m_LightGuardRadius = 0;
    public float m_ExpandLightGuardTime = 0.01f;

    private IEnumerator m_LightGuardEnumerator;

    public void Enable()
    {
        m_LightGuardEnumerator = GrowLightGuard();
        StartCoroutine(m_LightGuardEnumerator);
    }

    private IEnumerator GrowLightGuard()
    {
        float t = 0;

        Collider c = GetComponentInChildren<Collider>();

        m_LightGuardVisuals.Setup();
        m_LightGuardVisuals.transform.position = new Vector3(m_Center.transform.position.x, m_LightGuardVisuals.transform.position.y, m_Center.transform.position.z);

        float scale;
        while ((t += Time.deltaTime) < m_ExpandLightGuardTime)
        {
            scale = t / m_ExpandLightGuardTime * m_LightGuardRadius;
            m_LightGuardVisuals.ScaleUp(scale);
            if (c != null)
            {
                c.transform.localScale = new Vector3(scale * 2, scale * 2, scale * 2);   
            }
            yield return null;
        }

        scale = m_LightGuardRadius;
        m_LightGuardVisuals.ScaleUp(scale);
        if (c != null)
        {
            c.transform.localScale = new Vector3(scale * 2, scale * 2, scale * 2);
        }

    }

    public void DetachVisualsFromParent()
    {
        m_LightGuardVisuals.DetachFromParent();
    }

    public void ReattachVisualsToParent()
    {
        m_LightGuardVisuals.ReattachToParent();
    }
}

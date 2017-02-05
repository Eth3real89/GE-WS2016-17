using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGuard : MonoBehaviour {

    public BlastWaveVisuals m_LightGuardVisuals;

    public GameObject m_Boss;

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
        m_LightGuardVisuals.transform.position = new Vector3(m_Boss.transform.position.x, m_LightGuardVisuals.transform.position.y, m_Boss.transform.position.z);

        while ((t += Time.deltaTime) < m_ExpandLightGuardTime)
        {
            float scale = t / m_ExpandLightGuardTime * m_LightGuardRadius;
            m_LightGuardVisuals.ScaleUp(scale);
            if (c != null)
            {
                c.transform.localScale = new Vector3(scale * 2, c.transform.localScale.y, scale * 2);   
            }
            yield return null;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGuard : MonoBehaviour {

    public LightGuardVisuals m_LightGuardVisuals;

    public GameObject m_Center;

    public GameObject m_DissolveParticleEffect;

    public float m_LightGuardRadius = 0;
    public float m_ExpandLightGuardTime = 0.01f;

    private IEnumerator m_LightGuardEnumerator;

    private MeshRenderer m_MeshRenderer;

    public void Enable()
    {
        gameObject.SetActive(true);
        m_MeshRenderer = m_LightGuardVisuals.GetComponentInChildren<MeshRenderer>();
        m_MeshRenderer.material.SetFloat("_SliceAmount", 1.0f);

        m_LightGuardVisuals.GetComponentInChildren<ParticleSystem>().Stop();

        m_LightGuardEnumerator = GrowLightGuard();        

        StartCoroutine(m_LightGuardEnumerator);
    }

    public void Disable()
    {
        StartCoroutine(DissolveLightGuard());
    }

    private IEnumerator GrowLightGuard()
    {
        float t = 0.0f;

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

            m_MeshRenderer.material.SetFloat("_SliceAmount", 1.0f - t / m_ExpandLightGuardTime);

            yield return null;
        }

        m_MeshRenderer.material.SetFloat("_SliceAmount", 0.0f);

        scale = m_LightGuardRadius;
        m_LightGuardVisuals.ScaleUp(scale);
        if (c != null)
        {
            c.transform.localScale = new Vector3(scale * 2, scale * 2, scale * 2);
        }
    }

    private IEnumerator DissolveLightGuard() 
    {
        float t = 0.0f;

        m_LightGuardVisuals.GetComponentInChildren<ParticleSystem>().Play();

        while ((t += Time.deltaTime) < 1.5)
        {
            m_MeshRenderer.material.SetFloat("_SliceAmount", t  / 1.5f);

            yield return null;
        }

        m_LightGuardVisuals.GetComponentInChildren<ParticleSystem>().Stop();

        m_MeshRenderer.material.SetFloat("_SliceAmount", 1.0f);

        gameObject.SetActive(false);
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

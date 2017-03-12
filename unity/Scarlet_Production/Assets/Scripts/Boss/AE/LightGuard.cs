using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGuard : MonoBehaviour {

    protected LightGuardVisuals m_LightGuardVisuals;

    public GameObject m_Center;

    public float m_LightGuardRadius = 0;
    public float m_ExpandLightGuardTime = 0.01f;

    private IEnumerator m_LightGuardEnumerator;

    private MeshRenderer m_MeshRenderer;

    public void Enable()
    {
        gameObject.SetActive(true);
        LoadPrefab();

        m_MeshRenderer = m_LightGuardVisuals.GetComponentInChildren<MeshRenderer>();
        m_MeshRenderer.material.SetFloat("_SliceAmount", 1.0f);

        ParticleSystem ps = m_LightGuardVisuals.GetComponentInChildren<ParticleSystem>();

        if(ps != null)
        {
            ps.Stop();
        }

        m_LightGuardEnumerator = GrowLightGuard();

        StartCoroutine(m_LightGuardEnumerator);
    }

    protected void LoadPrefab()
    {
        if (m_LightGuardVisuals == null)
        {
            m_LightGuardVisuals = Instantiate(AEPrefabManager.Instance.m_LightGuardPrefab, this.transform).GetComponent<LightGuardVisuals>();
            m_LightGuardVisuals.transform.localPosition = new Vector3(0, 0, 0);
            m_LightGuardVisuals.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void Disable(bool destroyAfter = false)
    {
        if (m_LightGuardVisuals != null)
            StartCoroutine(DissolveLightGuard(destroyAfter));

        if (m_LightGuardVisuals != null)
            m_LightGuardVisuals.GetComponentInChildren<Collider>().enabled = false;
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

    private IEnumerator DissolveLightGuard(bool destroyAfter = false) 
    {
        float t = 0.0f;

        ParticleSystem ps = m_LightGuardVisuals.GetComponentInChildren<ParticleSystem>();

        if(ps != null)
        {
            ps.Play();
        }
           
        while ((t += Time.deltaTime) < 1.5)
        {
            m_MeshRenderer.material.SetFloat("_SliceAmount", t  / 1.5f);

            yield return null;
        }

        if(ps != null)
        {
            ps.Stop();
        }

        m_MeshRenderer.material.SetFloat("_SliceAmount", 1.0f);

        if (m_LightGuardVisuals != null && destroyAfter)
        {
            Destroy(m_LightGuardVisuals);
            m_LightGuardVisuals = null;
        }
        
        gameObject.SetActive(false);
    }

    public void DetachVisualsFromParent()
    {
        if (m_LightGuardVisuals != null)
            m_LightGuardVisuals.DetachFromParent();
    }

    public void ReattachVisualsToParent()
    {
        if (m_LightGuardVisuals != null)
            m_LightGuardVisuals.ReattachToParent();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadePlayableEffect : PlayableEffect
{
    public GameObject m_Prefab;
    protected GameObject m_Instance;

    public float m_Time = 1f;

    public override void Play(Vector3 position = default(Vector3))
    {
        m_Instance = Instantiate(m_Prefab);
        m_Instance.transform.position = position * 1f;

        m_Instance.gameObject.SetActive(true);

        m_Instance.gameObject.GetComponentInChildren<Light>().enabled = true;

        StartCoroutine(HideExplosionLight());
        StartCoroutine(DestroyAfter(m_Time));
    }

    private IEnumerator HideExplosionLight()
    {
        yield return new WaitForSeconds(0.1f);

        m_Instance.gameObject.GetComponentInChildren<Light>().enabled = false;
    }

    protected virtual IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);

        if (m_Instance != null)
        {
            Destroy(m_Instance);
        }
    }

    public override void Hide()
    {

        if (m_Instance != null)
        {
            Destroy(m_Instance);
        }
    }
}

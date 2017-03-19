using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHitVisualisation : MonoBehaviour {

    public GameObject m_GroundHitEffects;
    public Vector3 m_Position; // = new Vector3(-2.0f, 0.0f, 5.8f);

    private Component[] m_Comps;

    // Use this for initialization
    void Start()
    {
        m_Comps = m_GroundHitEffects.GetComponentsInChildren<ParticleSystem>(true);
        
        m_GroundHitEffects.transform.localPosition = m_Position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnScarletHitsGround()
    {
        foreach (ParticleSystem ps in m_Comps)
        {
            ps.Play();
        }

        StartCoroutine(StopParticleSystems());
    }

    private IEnumerator StopParticleSystems()
    {
        yield return new WaitForSeconds(2.0f);

        foreach (ParticleSystem ps in m_Comps)
        {
            ps.Stop();
        }
        
        m_GroundHitEffects.transform.localPosition = m_Position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmourFairyGroundHitVisualisation : MonoBehaviour {

    public GameObject m_GroundHitEffects;
    public Vector3 m_Position; // = new Vector3(-2.0f, 0.0f, 5.8f);

    private Component[] m_Comps;

	// Use this for initialization
	void Start () {
        m_Comps = m_GroundHitEffects.GetComponentsInChildren<ParticleSystem>(true);

        m_GroundHitEffects.transform.parent = transform;

        m_GroundHitEffects.transform.localPosition = m_Position;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // play the effects for sword or shield hitting the ground
    void OnAttackHitsGround() 
    {
        m_GroundHitEffects.transform.parent = null;

        foreach(ParticleSystem ps in m_Comps)
        {
            ps.Play();
        }

        StartCoroutine(StopParticleSystems()); 
    }

    private IEnumerator StopParticleSystems() 
    {
        yield return new WaitForSeconds(2.0f);

        foreach(ParticleSystem ps in m_Comps)
        {
            ps.Stop();
        }

        m_GroundHitEffects.transform.parent = transform;
        m_GroundHitEffects.transform.localPosition = m_Position;
    }
}

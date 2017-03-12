using DigitalRuby.PyroParticles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChurchFireController : MonoBehaviour {

    [Tooltip("Each entry of the array has to contain the lights to light up at once.")]
    public GameObject[] m_FireContainers;
    [Tooltip("Time to wait till next fire is light up.")]
    public float m_WaitTillNext = 2f;

    private bool m_LightUpNext = false;
    private int m_FireIndex = 0;
    private float m_WaitTillNextCurrent;

    // -------- START - DAS ENTFERNEN WENN DAS EXTERN GETRIGGERT WIRD -----------
    private float time = 5f;
    // -------- ENDE  - DAS ENTFERNEN WENN DAS EXTERN GETRIGGERT WIRD -----------

    // Use this for initialization
    void Start () {
        m_WaitTillNextCurrent = m_WaitTillNext;
    }
	
	// Update is called once per frame
	void Update () {
		if(m_LightUpNext && m_FireIndex < m_FireContainers.Length)
        {
            m_WaitTillNextCurrent -= Time.deltaTime;
            if(m_WaitTillNextCurrent <= 0)
            {
                m_WaitTillNextCurrent = m_WaitTillNext;
                m_LightUpNext = false;
                StartCoroutine(LightUpFires());
            }
        }
        // -------- START - DAS ENTFERNEN WENN DAS EXTERN GETRIGGERT WIRD -----------
        else if (m_FireIndex < m_FireContainers.Length)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                OnLightUpFires();
            }
        }
        // -------- ENDE  - DAS ENTFERNEN WENN DAS EXTERN GETRIGGERT WIRD -----------
    }

    IEnumerator LightUpFires()
    {
        FireConstantBaseScript[] scripts = m_FireContainers[m_FireIndex].GetComponentsInChildren<FireConstantBaseScript>();
        foreach(FireConstantBaseScript script in scripts)
        {
            script.StartFire();
            yield return null;
        }
        m_FireIndex++;
        m_LightUpNext = true;
    }

    public void OnLightUpFires()
    {
        m_LightUpNext = true;
    }
}

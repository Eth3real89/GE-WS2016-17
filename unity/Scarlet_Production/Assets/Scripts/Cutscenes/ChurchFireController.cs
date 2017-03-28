using DigitalRuby.PyroParticles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChurchFireController : MonoBehaviour {

    [Tooltip("Each entry of the array has to contain the lights to light up at once.")]
    public GameObject[] m_FireContainers;
    [Tooltip("Time to wait till next fire is light up.")]
    public float m_WaitTillNext = 2f;

    [Tooltip("GameObjects that should change color additional to Fire Containers.")]
    public GameObject[] m_AdditionalToColorChange;

    private List<ParticleSystem> m_ParticleSystems = new List<ParticleSystem>();
    private List<Light> m_Lights = new List<Light>();

    private bool m_LightUpNext = false;
    private bool m_StartChangingToNormal = false;
    private bool m_PutOutFires = false;

    private int m_FireIndex = 0;
    private float m_WaitTillNextCurrent;
    private List<FireConstantBaseScript> m_FireBaseScripts = new List<FireConstantBaseScript>();

    // -------- START - DAS ENTFERNEN WENN DAS EXTERN GETRIGGERT WIRD -----------
    public float time = 5f;
    // -------- ENDE  - DAS ENTFERNEN WENN DAS EXTERN GETRIGGERT WIRD -----------

    public bool m_SkipEverythingAndStartImmediately = false;

    // Use this for initialization
    void Start () {
        if (m_SkipEverythingAndStartImmediately)
        {
            StartImmediately();
            return;
        }

        m_WaitTillNextCurrent = m_WaitTillNext;

        foreach (GameObject container in m_FireContainers)
        {
            m_ParticleSystems.AddRange(container.GetComponentsInChildren<ParticleSystem>());
            m_Lights.AddRange(container.GetComponentsInChildren<Light>());
        }
        foreach (GameObject container in m_AdditionalToColorChange)
        {
            m_ParticleSystems.AddRange(container.GetComponentsInChildren<ParticleSystem>());
            m_Lights.AddRange(container.GetComponentsInChildren<Light>());
        }

    }

    // Update is called once per frame
    void Update () {
        if (m_SkipEverythingAndStartImmediately)
            return;

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
        else if(m_StartChangingToNormal)
        {
            m_StartChangingToNormal = false;
            Color newFull = new Color(1, 1, 1, 1);
            Color newLightColor = new Color(1, 0.92f, 0.8f, 1);
            Color newTransparent = new Color(1, 1, 1, 0.3f);
            foreach (ParticleSystem particles in m_ParticleSystems)
            {
                if(particles.name == "WallOfFireSmoke")
                {
                    particles.startColor = newTransparent;
                } else
                {
                    particles.startColor = newFull;
                }
            }
            foreach(Light light in m_Lights)
            {
                light.color = newLightColor;
            }
            //OnPutOutFires();
        }
        else if(m_PutOutFires)
        {
            m_PutOutFires = false;

            foreach(GameObject container in m_AdditionalToColorChange)
            {
                m_FireBaseScripts.Add(container.GetComponentInChildren<FireConstantBaseScript>());
            }
            foreach(FireConstantBaseScript fireScript in m_FireBaseScripts)
            {
                fireScript.Stop();
            }
        }
    }
    

    IEnumerator LightUpFires()
    {
        FireConstantBaseScript[] scripts = m_FireContainers[m_FireIndex].GetComponentsInChildren<FireConstantBaseScript>();

        foreach (FireConstantBaseScript script in scripts)
        {
            m_FireBaseScripts.Add(script);
            script.StartFire();
            yield return null;
        }
        m_FireIndex++;
        m_LightUpNext = true;

        //if(m_FireIndex == m_FireContainers.Length)
        //{
        //    OnChangeColorsToNormal();
        //}
    }

    protected void StartImmediately()
    {
        for (int i = 0; i < m_FireContainers.Length; i++)
        {
            FireConstantBaseScript[] scripts = m_FireContainers[i].GetComponentsInChildren<FireConstantBaseScript>();

            foreach (ParticleSystem ps in m_FireContainers[i].GetComponentsInChildren<ParticleSystem>())
            {
                var x = ps.main;
                x.prewarm = true;
                ps.Play();
            }

            foreach (FireConstantBaseScript script in scripts)
            {
                m_FireBaseScripts.Add(script);
                script.StartFire();
            }
        }
    }

    public void OnLightUpFires()
    {
        m_LightUpNext = true;
    }

    public void OnChangeColorsToNormal()
    {
        m_StartChangingToNormal = true;
    }

    public void OnPutOutFires()
    {
        m_PutOutFires = true;
    }

}

﻿using System.Linq;
using System.Threading;
using UnityEngine;

public class ControlAngelVisualisation : MonoBehaviour {

    public GameObject m_LeftHand;
    public GameObject m_RightHand;
    public GameObject m_SpecialAttack;
    public GameObject m_SpecialAttack_DOWN;
    public GameObject m_SpecialLandVisualisation;

    public int m_LowParticleEmissionSmoke;
    public int m_HighParticleEmissionSmoke;
    public int m_LowParticleEmissionDirection;
    public int m_HighParticleEmissionDirection;
    
    public Color[] trailColorLight;
    public Color[] trailColorFull;

    private MeleeWeaponTrail[] m_Trails_Right;
    private ParticleSystem[] m_Particles_Right;

    private MeleeWeaponTrail[] m_SpecialTrails;
    private ParticleSystem[] m_SpecialParticles_UP;
    private ParticleSystem[] m_SpecialParticles_DOWN;

    private Light m_Light;

    private ParticleSystem[] m_SpecialParticlesLand;

    private float t = 0.5f;

    private float m_TimeStartLight = 0.5f;
    private float m_TimeImpactLight = 1.8f;

    private bool m_StartLightOn = false;
    private bool m_ImpactLightOn = false;

    // Use this for initialization
    void Start()
    {
        m_Light = m_SpecialLandVisualisation.GetComponent<Light>();

        m_SpecialTrails = m_SpecialAttack.GetComponentsInChildren<MeleeWeaponTrail>(true);
        m_Trails_Right = m_RightHand.GetComponentsInChildren<MeleeWeaponTrail>(true);
        

        m_Trails_Right = m_Trails_Right.Where(val => !m_SpecialTrails.Contains(val)).ToArray();



        m_SpecialParticlesLand = m_SpecialLandVisualisation.GetComponentsInChildren<ParticleSystem>();
        m_SpecialParticles_UP = m_SpecialAttack.GetComponentsInChildren<ParticleSystem>(true);
        m_SpecialParticles_DOWN = m_SpecialAttack_DOWN.GetComponentsInChildren<ParticleSystem>(true);
        m_Particles_Right = m_RightHand.GetComponentsInChildren<ParticleSystem>(true);
    }

    public void OnWeaponTipChanged(GameObject newTip)
    {
        try
        {
            m_RightHand = newTip;
            m_RightHand = newTip.transform.FindChild("NormalAttack").gameObject;
            m_Trails_Right = m_RightHand.GetComponentsInChildren<MeleeWeaponTrail>(true);
            m_Particles_Right = m_RightHand.GetComponentsInChildren<ParticleSystem>(true);
        }
        catch { }
    }

    void Update()
    {
        if(m_SpecialParticlesLand.First().isPlaying)
        {
            t -= Time.deltaTime;
            if(t <= 0)
            {
                foreach(ParticleSystem particles in m_SpecialParticlesLand)
                {
                    particles.Stop();
                }
                t = 0.5f;
            }
        }

        if(m_Light.intensity != 0)
        {
            if(m_StartLightOn)
            {
                m_TimeStartLight -= Time.deltaTime;
                if (m_TimeStartLight <= 0)
                {
                    m_Light.intensity = 0;
                    m_Light.range = 6;
                    m_TimeStartLight = 0.5f;
                    m_StartLightOn = false;
                }
                else
                {
                    m_Light.intensity = Mathf.Lerp(0, 5, m_TimeStartLight * 2);
                }
            }
            else if (m_ImpactLightOn)
            {
                m_TimeImpactLight -= Time.deltaTime;
                if (m_TimeImpactLight <= 0)
                {
                    m_Light.intensity = 0;
                    m_TimeImpactLight = 1.8f;
                    m_Light.range = 4;
                    m_ImpactLightOn = false;
                }
                else
                {
                    m_Light.intensity = Mathf.Lerp(0, 8, m_TimeImpactLight * 2);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// 0: Axe
    /// 1: Crossbow
    /// 2: Halberd
    /// 3: Hammer
    /// 4: Scythe
    /// 5: Spear
    /// </summary>
    public void WeaponAttackStart(int weaponIndex)
    {
        foreach (MeleeWeaponTrail trail in m_Trails_Right)
        {
            trail.Colors = trailColorFull;
        }

        foreach (ParticleSystem particles in m_Particles_Right)
        {
            ParticleSystem.EmissionModule emission = particles.emission;
            if (particles.name.Equals("particle_smoke"))
            {
                emission.rateOverDistance = m_HighParticleEmissionSmoke;
            }
            else if (particles.name.Equals("particle_direction"))
            {
                emission.rateOverDistance = m_HighParticleEmissionDirection;
            }
        }
    }

    public void WeaponAttackEnd(int weaponIndex)
    {
        foreach (MeleeWeaponTrail trail in m_Trails_Right)
        {
            trail.Colors = trailColorLight;
        }

        foreach (ParticleSystem particles in m_Particles_Right)
        {
            ParticleSystem.EmissionModule emission = particles.emission;
            if (particles.name.Equals("particle_smoke"))
            {
                emission.rateOverDistance = m_LowParticleEmissionSmoke;
            }
            else if (particles.name.Equals("particle_direction"))
            {
                emission.rateOverDistance = m_LowParticleEmissionDirection;
            }
        }
    }

    public void SpecialAttackDownStart()
    {
        foreach (ParticleSystem particles in m_SpecialParticles_DOWN)
        {
            particles.Play();
        }
    }

    public void SpecialAttackStart()
    {
        m_Light.range = 4;
        m_Light.intensity = 5;
        m_StartLightOn = true;
        foreach (ParticleSystem particles in m_SpecialParticles_UP)
        {
            particles.Play();
        }
        foreach (MeleeWeaponTrail trail in m_SpecialTrails)
        {
            trail.Emit = true;
        }
    }

    public void SpecialAttackEnd()
    {
        foreach (ParticleSystem system in m_SpecialParticlesLand)
        {
            system.Play();
        }
        foreach (ParticleSystem particles in m_SpecialParticles_UP)
        {
            particles.Stop();
        }
        foreach (ParticleSystem particles in m_SpecialParticles_DOWN)
        {
            particles.Stop();
        }
        foreach (MeleeWeaponTrail trail in m_SpecialTrails)
        {
            trail.Emit = false;
        }
        m_Light.range = 6;
        m_Light.intensity = 8;
        m_ImpactLightOn = true;
    }
}

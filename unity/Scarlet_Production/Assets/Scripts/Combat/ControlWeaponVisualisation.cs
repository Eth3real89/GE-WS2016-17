using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlWeaponVisualisation : MonoBehaviour {

    public GameObject m_LeftHand;
    public GameObject m_RightHand;

    public GameObject[] m_WeaponsLeft;
    public GameObject[] m_WeaponsRight;

    public Color[] trailColor;

    public int m_LowParticleEmissionSmoke;
    public int m_HighParticleEmissionSmoke;
    public int m_LowParticleEmissionDirection;
    public int m_HighParticleEmissionDirection;


    private Color[] trailColorLight;
    private Color[] trailColorFull;

    private MeleeWeaponTrail[] m_Trails_Left;

    private MeleeWeaponTrail[] m_Trails_Right_Small;
    private MeleeWeaponTrail[] m_Trails_Right_Big;

    private ParticleSystem[] m_Particles_Left;
    private ParticleSystem[] m_Particles_Right_Small;
    private ParticleSystem[] m_Particles_Right_Big;

    // Use this for initialization
    void Start()
    {
        trailColorLight = new Color[trailColor.Length];
        trailColorFull = new Color[trailColor.Length];
        for (var i = 0; i < trailColor.Length; i++)
        {
            trailColorFull[i] = trailColor[i];
            trailColor[i].a = 0.3f;
            trailColorLight[i] = trailColor[i];
        }

        m_Trails_Left = m_LeftHand.GetComponentsInChildren<MeleeWeaponTrail>(true);
        
        m_Trails_Right_Small = m_WeaponsRight[0].GetComponentsInChildren<MeleeWeaponTrail>(true);
        m_Trails_Right_Big = m_WeaponsRight[1].GetComponentsInChildren<MeleeWeaponTrail>(true);

        m_Particles_Left = m_LeftHand.GetComponentsInChildren<ParticleSystem>(true);

        m_Particles_Right_Small = m_WeaponsRight[0].GetComponentsInChildren<ParticleSystem>(true);
        m_Particles_Right_Big = m_WeaponsRight[1].GetComponentsInChildren<ParticleSystem>(true);
        
    }

    public void EnableVisualisationLeft()
    {
        foreach (ParticleSystem particles in m_Particles_Left)
        {
            ParticleSystem.EmissionModule emission = particles.emission;
            if (particles.name.Equals("particle_smoke_big"))
            {
                emission.rateOverDistance = m_HighParticleEmissionSmoke;
            }
            else if (particles.name.Equals("particle_direction_big"))
            {
                emission.rateOverDistance = m_HighParticleEmissionDirection;
            }
        }
        foreach (MeleeWeaponTrail trail in m_Trails_Left)
        {
            trail.Colors = trailColorFull;
        }
    }

    public void EnableVisualisationRight()
    {
        if(m_WeaponsRight[0].activeSelf)
        {
            foreach (ParticleSystem particles in m_Particles_Right_Small)
            {
                ParticleSystem.EmissionModule emission = particles.emission;
                if (particles.name.Equals("particle_smoke_big"))
                {
                    emission.rateOverDistance = m_HighParticleEmissionSmoke;
                }
                else if (particles.name.Equals("particle_direction_big"))
                {
                    emission.rateOverDistance = m_HighParticleEmissionDirection;
                }
            }
            foreach (MeleeWeaponTrail trail in m_Trails_Right_Small)
            {
                trail.Colors = trailColorFull;
            }
        } else
        {
            foreach (ParticleSystem particles in m_Particles_Right_Big)
            {
                ParticleSystem.EmissionModule emission = particles.emission;
                if (particles.name.Equals("particle_smoke_big"))
                {
                    emission.rateOverDistance = m_HighParticleEmissionSmoke;
                }
                else if (particles.name.Equals("particle_direction_big"))
                {
                    emission.rateOverDistance = m_HighParticleEmissionDirection;
                }
            }
            foreach (MeleeWeaponTrail trail in m_Trails_Right_Big)
            {
                trail.Colors = trailColorFull;
            }
        }
    }


    public void DisableVisualisationLeft()
    {
        foreach (ParticleSystem particles in m_Particles_Left)
        {
            ParticleSystem.EmissionModule emission = particles.emission;
            if (particles.name.Equals("particle_smoke_big"))
            {
                emission.rateOverDistance = m_LowParticleEmissionSmoke;
            }
            else if (particles.name.Equals("particle_direction_big"))
            {
                emission.rateOverDistance = m_LowParticleEmissionDirection;
            }
        }
        foreach (MeleeWeaponTrail trail in m_Trails_Left)
        {
            trail.Colors = trailColorLight;

        }

    }


    public void DisableVisualisationRight()
    {
        if (m_WeaponsRight[0].activeSelf)
        {
            foreach (ParticleSystem particles in m_Particles_Right_Small)
            {
                ParticleSystem.EmissionModule emission = particles.emission;
                if (particles.name.Equals("particle_smoke_big"))
                {
                    emission.rateOverDistance = m_LowParticleEmissionSmoke;
                }
                else if (particles.name.Equals("particle_direction_big"))
                {
                    emission.rateOverDistance = m_LowParticleEmissionDirection;
                }
            }
            foreach (MeleeWeaponTrail trail in m_Trails_Right_Small)
            {
                trail.Colors = trailColorLight;
            }
        }
        else
        {
            foreach (ParticleSystem particles in m_Particles_Right_Big)
            {
                ParticleSystem.EmissionModule emission = particles.emission;
                if (particles.name.Equals("particle_smoke_big"))
                {
                    emission.rateOverDistance = m_LowParticleEmissionSmoke;
                }
                else if (particles.name.Equals("particle_direction_big"))
                {
                    emission.rateOverDistance = m_LowParticleEmissionDirection;
                }
            }
            foreach (MeleeWeaponTrail trail in m_Trails_Right_Big)
            {
                trail.Colors = trailColorLight;
            }
        }
    }

    // handside: 0=right 1=left 2=both
    public void AttackWeaponStart(int handSide)
    {
        if (handSide == 0)
        {
            EnableVisualisationRight();
        }
        else if (handSide == 1)
        {
            EnableVisualisationLeft();
        }
        else if (handSide == 2)
        {
            EnableVisualisationRight();
            EnableVisualisationLeft();
        }
    }

    //both hands disable
    public void AttackWeaponEnd()
    {
        DisableVisualisationRight();
        DisableVisualisationLeft();
    }

}

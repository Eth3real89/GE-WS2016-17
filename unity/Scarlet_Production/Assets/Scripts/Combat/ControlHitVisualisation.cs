using UnityEngine;

/// <summary>
/// Attach to Character to control visualisation of hits. 
/// </summary>
public class ControlHitVisualisation : MonoBehaviour {

    public GameObject m_LeftHand;
    public GameObject m_RightHand;

    public Color[] trailColor;

    public int m_LowParticleEmissionSmoke;
    public int m_HighParticleEmissionSmoke;
    public int m_LowParticleEmissionDirection;
    public int m_HighParticleEmissionDirection;

    private Color[] trailColorLight;
    private Color[] trailColorFull;

    private MeleeWeaponTrail[] m_Trails_Left;
    private MeleeWeaponTrail[] m_Trails_Right;
    private ParticleSystem[] m_Particles_Left;
    private ParticleSystem[] m_Particles_Right;

    // Use this for initialization
    void Start () {
        trailColorLight = new Color[trailColor.Length];
        trailColorFull = new Color[trailColor.Length];
        for (var i = 0; i< trailColor.Length; i++)
        {
            trailColorFull[i] = trailColor[i];
            trailColor[i].a = 0.5f;
            trailColorLight[i] = trailColor[i];
        }

        m_Trails_Left = m_LeftHand.GetComponentsInChildren<MeleeWeaponTrail>(true);
        m_Trails_Right = m_RightHand.GetComponentsInChildren<MeleeWeaponTrail>(true);
        
        m_Particles_Left = m_LeftHand.GetComponentsInChildren<ParticleSystem>(true);
        m_Particles_Right = m_RightHand.GetComponentsInChildren<ParticleSystem>(true);
    }

    public void EnableVisualisationLeft()
    {
        foreach (ParticleSystem particles in m_Particles_Left)
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
        foreach (MeleeWeaponTrail trail in m_Trails_Left)
        {
            trail.Colors = trailColorFull;
        }
    }

    public void EnableVisualisationRight()
    {
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
        foreach (MeleeWeaponTrail trail in m_Trails_Right)
        {
            trail.Colors = trailColorFull;

        }
    }


    public void DisableVisualisationLeft()
    {
        foreach (ParticleSystem particles in m_Particles_Left)
        {
            ParticleSystem.EmissionModule emission = particles.emission;
            if (particles.name.Equals("particle_smoke"))
            {
                emission.rateOverDistance = m_LowParticleEmissionSmoke;
            }
            else if(particles.name.Equals("particle_direction"))
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
        foreach (MeleeWeaponTrail trail in m_Trails_Right)
        {
            trail.Colors = trailColorLight;
        }
    }

    // handside: 0=right 1=left 2=both
    public void AttackAnimationStart(int handSide)
    {
        if(handSide == 0)
        {
            EnableVisualisationRight();
        } else if(handSide == 1)
        {
            EnableVisualisationLeft();
        } else if(handSide == 2)
        {
            EnableVisualisationRight();
            EnableVisualisationLeft();
        }
    }

    //both hands disable
    public void AttackAnimationEnd()
    {
        DisableVisualisationRight();
        DisableVisualisationLeft();
    }

}

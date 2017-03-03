using System.Linq;
using System.Threading;
using UnityEngine;

public class ControlAngelVisualisation : MonoBehaviour {

    public GameObject m_LeftHand;
    public GameObject m_RightHand;
    public GameObject m_SpecialAttack;
    public GameObject m_SpecialLandVisualisation;

    private MeleeWeaponTrail[] m_Trails_Left;
    private MeleeWeaponTrail[] m_Trails_Right;
    private ParticleSystem[] m_Particles_Left;
    private ParticleSystem[] m_Particles_Right;

    private MeleeWeaponTrail[] m_SpecialTrails;
    private ParticleSystem[] m_SpecialParticles;
    private ParticleSystem[] m_SpecialParticlesLand;

    private float t = 500;

    // Use this for initialization
    void Start()
    {
        m_Trails_Left = m_LeftHand.GetComponentsInChildren<MeleeWeaponTrail>(true);

        m_SpecialTrails = m_SpecialAttack.GetComponentsInChildren<MeleeWeaponTrail>(true);
        m_Trails_Right = m_RightHand.GetComponentsInChildren<MeleeWeaponTrail>(true);
        

        m_Trails_Right = m_Trails_Right.Where(val => !m_SpecialTrails.Contains(val)).ToArray();


        m_Particles_Left = m_LeftHand.GetComponentsInChildren<ParticleSystem>(true);

        m_SpecialParticlesLand = m_SpecialLandVisualisation.GetComponentsInChildren<ParticleSystem>();
        m_SpecialParticles = m_SpecialAttack.GetComponentsInChildren<ParticleSystem>(true);
        m_Particles_Right = m_RightHand.GetComponentsInChildren<ParticleSystem>(true);
        
    }

    void Update()
    {
        if(m_SpecialParticlesLand.First().isPlaying)
        {
            t -= Time.deltaTime;
            if(t <= 0)
            {
                foreach(ParticleSystem system in m_SpecialParticlesLand)
                {
                    system.Stop();
                }
                t = 500;
            }
        }
    }

    public void EnableVisualisationLeft()
    {
        foreach (ParticleSystem particles in m_Particles_Left)
        {
            particles.Play();
        }
        foreach (MeleeWeaponTrail trail in m_Trails_Left)
        {
            trail.Emit = true;
        }
    }

    public void EnableVisualisationRight()
    {
        foreach (ParticleSystem particles in m_Particles_Right)
        {
            particles.Play();
        }
        foreach (MeleeWeaponTrail trail in m_Trails_Right)
        {
            trail.Emit = true;
        }
    }


    public void DisableVisualisationLeft()
    {
        foreach (ParticleSystem particles in m_Particles_Left)
        {
            particles.Stop();
        }
        foreach (MeleeWeaponTrail trail in m_Trails_Left)
        {
            trail.Emit = false;
        }

    }

    public void DisableVisualisationRight()
    {
        foreach (ParticleSystem particles in m_Particles_Right)
        {
            particles.Stop();
        }
        foreach (MeleeWeaponTrail trail in m_Trails_Right)
        {
            trail.Emit = false;
        }
    }

    public void EnableSpecialVisualisation()
    {
        foreach (ParticleSystem particles in m_SpecialParticles)
        {
            particles.Play();
        }
        foreach (MeleeWeaponTrail trail in m_SpecialTrails)
        {
            trail.Emit = true;
        }
    }

    public void DisableSpecialVisualisation()
    {
        foreach (ParticleSystem system in m_SpecialParticlesLand)
        {
            system.Play();
        }
        foreach (ParticleSystem particles in m_SpecialParticles)
        {
            particles.Stop();
        }
        foreach (MeleeWeaponTrail trail in m_SpecialTrails)
        {
            trail.Emit = false;
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
    
    public void SpecialAttackStart()
    {
        EnableSpecialVisualisation();
    }

    public void SpecialAttackEnd()
    {
        DisableSpecialVisualisation();
    }

    //both hands disable
    public void AttackWeaponEnd()
    {
        DisableVisualisationRight();
        DisableVisualisationLeft();
    }
}

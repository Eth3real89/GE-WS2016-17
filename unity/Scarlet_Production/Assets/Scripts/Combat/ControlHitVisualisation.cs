using UnityEngine;

/// <summary>
/// Attach to Character to control visualisation of hits. 
/// </summary>
public class ControlHitVisualisation : MonoBehaviour {

    public GameObject m_LeftHand;
    public GameObject m_RightHand;

    private MeleeWeaponTrail[] m_Trails_Left;
    private MeleeWeaponTrail[] m_Trails_Right;
    private ParticleSystem[] m_Particles_Left;
    private ParticleSystem[] m_Particles_Right;

    // Use this for initialization
    void Start () {
        m_Trails_Left = m_LeftHand.GetComponentsInChildren<MeleeWeaponTrail>(true);
        m_Trails_Right = m_RightHand.GetComponentsInChildren<MeleeWeaponTrail>(true);
        
        m_Particles_Left = m_LeftHand.GetComponentsInChildren<ParticleSystem>(true);
        m_Particles_Right = m_RightHand.GetComponentsInChildren<ParticleSystem>(true);
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

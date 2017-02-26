using UnityEngine;

/// <summary>
/// Attach to Character to control visualisation of hits. 
/// </summary>
public class ControlHitVisualisation : MonoBehaviour {

    private TrailRenderer[] m_Trails;
    private ParticleSystem[] m_Particles;

    // Use this for initialization
    void Start () {
        m_Trails = gameObject.transform.Find("Model").GetComponentsInChildren<TrailRenderer>();
        m_Particles = gameObject.transform.Find("Model").GetComponentsInChildren<ParticleSystem>();
    }

    public void EnableVisualisation()
    {
        foreach (ParticleSystem particles in m_Particles)
        {
            particles.Play();
        }
        foreach (TrailRenderer trail in m_Trails)
        {
            trail.enabled = true;
        }
    }

    public void DisableVisualisation()
    {
        foreach (ParticleSystem particles in m_Particles)
        {
            particles.Stop();
        }
        foreach (TrailRenderer trail in m_Trails)
        {
            trail.enabled = false;
        }
    }

}

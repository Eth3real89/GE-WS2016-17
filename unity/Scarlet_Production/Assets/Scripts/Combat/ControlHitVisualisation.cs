using UnityEngine;

/// <summary>
/// Attach to Character to control visualisation of hits. 
/// </summary>
public class ControlHitVisualisation : MonoBehaviour {

    private TrailRenderer[] m_Trails;

    // Use this for initialization
    void Start () {
        m_Trails = gameObject.transform.Find("Model").GetComponentsInChildren<TrailRenderer>();
    }

    public void EnableVisualisation()
    {
        foreach (TrailRenderer trail in m_Trails)
        {
            trail.enabled = true;
        }
    }

    public void DisableVisualisation()
    {
        foreach (TrailRenderer trail in m_Trails)
        {
            trail.enabled = false;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrackingBehaviour : MonoBehaviour
{
    public Transform m_Player;
    private CameraTracking m_CameraTracking;

    public void Start()
    {
        m_CameraTracking = Camera.main.GetComponent<CameraTracking>();
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_CameraTracking.m_TrackingBehaviour = this;
        }
    }

    public abstract Vector3 CalculateCameraPosition();
    public abstract Quaternion CalculateCameraRotation();
}

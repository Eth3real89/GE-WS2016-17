using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public GameObject m_Light;

    private Camera m_Camera;
    private OnCollectibleVFX m_OnCollectibleVFX;

    private void Start()
    {
        m_Camera = Camera.main;
        m_OnCollectibleVFX = Camera.main.GetComponent<OnCollectibleVFX>();
    }

    private void Update()
    {
        HandleCollectibleVisualization();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            m_Light.SetActive(false);
            gameObject.SetActive(false);
            m_OnCollectibleVFX.Deactivate();
            Destroy(this);
        }
    }

    private void HandleCollectibleVisualization()
    {
        RaycastHit hit;
        Physics.Raycast(m_Camera.transform.position, transform.position - m_Camera.transform.position, out hit);

        if (GetComponent<Renderer>().isVisible && hit.transform == transform)
        {
            Vector3 target = m_Camera.GetComponent<Camera>().WorldToScreenPoint(transform.position);
            m_OnCollectibleVFX.Activate(target);
        }
        else
        {
            m_OnCollectibleVFX.Deactivate();
        }
    }
}

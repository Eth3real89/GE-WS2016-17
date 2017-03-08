using System;
using UnityEngine;

public class PickUp : Interactor
{
    public GameObject[] m_Enables;
    public GameObject[] m_Disables;

    public GameObject[] m_Auras;

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

    public override void Interact()
    {
        base.Interact();
        GetComponentInChildren<UIItemPickupController>().OnItemPickedUp();
        foreach (GameObject aura in m_Auras)
        {
            aura.SetActive(true);
        }
        gameObject.SetActive(false);
        m_OnCollectibleVFX.Deactivate();
        OpenTheGates();
        EffectController.Instance.EmpoweredPeak();
        Destroy(this);
    }

    private void OpenTheGates()
    {
        foreach (GameObject go in m_Enables)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in m_Disables)
        {
            go.SetActive(false);
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

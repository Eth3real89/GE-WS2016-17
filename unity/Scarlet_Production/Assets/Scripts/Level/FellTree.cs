using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FellTree : Interactor
{
    public GameObject[] m_Objects;

    public GameObject m_Portal;

    public override void Interact()
    {
        if (!m_IsInteractible)
            return;

        m_IsInteractible = false;
        new FARQ().ClipName("treefalling").Location(transform).Play();
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponentInChildren<UIItemPickupController>().OnItemPickedUp();
        GetComponentInChildren<UIItemPickupController>().StopInteraction();
        StartCoroutine(SwitchStuffOff());
        StartCoroutine(EnableStartEffect());
        StartCoroutine(EnablePortal());

    }

    IEnumerator SwitchStuffOff()
    {
        yield return new WaitForSeconds(1f);
        foreach (GameObject go in m_Objects)
        {
            Destroy(go);
        }
    }

    IEnumerator EnableStartEffect()
    {
        yield return new WaitForSeconds(1f);

        m_Portal.GetComponentInChildren<ParticleSystem>().Play();
    }

    IEnumerator EnablePortal() 
    {
        yield return new WaitForSeconds(3f);
        m_Portal.GetComponent<MeshRenderer>().enabled = true;
    }
}

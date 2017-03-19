using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FellTree : Interactor
{
    public GameObject[] m_Objects;

    public override void Interact()
    {
        if (!m_IsInteractible)
            return;

        m_IsInteractible = false;
        new FARQ().ClipName("treefalling").Location(transform).Play();
        GetComponent<Rigidbody>().isKinematic = false;
        StartCoroutine(SwitchStuffOff());
    }

    IEnumerator SwitchStuffOff()
    {
        yield return new WaitForSeconds(1);
        foreach (GameObject go in m_Objects)
        {
            Destroy(go);
        }
    }
}

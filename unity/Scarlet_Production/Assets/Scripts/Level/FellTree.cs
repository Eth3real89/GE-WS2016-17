using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FellTree : Interactor
{
    public GameObject[] m_Objects;

    public override void Interact()
    {
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

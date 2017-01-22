using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisable : MonoBehaviour
{
    public GameObject[] m_ObjectsToEnable;
    public GameObject[] m_ObjectsToDisable;

    private void OnTriggerEnter(Collider other)
    {
        foreach (GameObject go in m_ObjectsToEnable)
        {
            go.SetActive(true);
        }
        foreach (GameObject go in m_ObjectsToDisable)
        {
            go.SetActive(false);
        }
    }
}

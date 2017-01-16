using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public GameObject m_Light;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_Light.SetActive(false);
            gameObject.SetActive(false);
            Destroy(this);
        }
    }
}

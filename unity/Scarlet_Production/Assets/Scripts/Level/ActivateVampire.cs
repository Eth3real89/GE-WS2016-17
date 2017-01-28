using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateVampire : MonoBehaviour
{
    public VampireFollow m_VampireFollow;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_VampireFollow.m_Active = true;
            Destroy(this);
        }
    }
}

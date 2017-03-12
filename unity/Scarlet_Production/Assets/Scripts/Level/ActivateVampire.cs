using SequencedActionCreator;
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
            SequencedActionController.Instance.PlayCutscene("Vampire");
        }
    }

    public void ActivateFollow()
    {
        m_VampireFollow.m_Active = true;
        Destroy(this);
    }
}

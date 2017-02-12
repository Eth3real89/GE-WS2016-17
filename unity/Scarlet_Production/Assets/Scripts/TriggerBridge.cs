using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBridge : Interactor
{
    public Rigidbody m_Bridge;

    public override void Interact()
    {
        m_Bridge.isKinematic = false;
    }
}

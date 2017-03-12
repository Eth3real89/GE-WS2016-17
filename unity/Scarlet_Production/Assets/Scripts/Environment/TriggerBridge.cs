using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBridge : Interactor
{
    public BridgeController m_BridgeController;

    public override void Interact()
    {
        m_BridgeController.Lower();
    }
}

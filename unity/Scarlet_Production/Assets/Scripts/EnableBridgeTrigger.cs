using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBridgeTrigger : Interactor
{
    public GameObject m_OtherInteractor;

    public override void Interact()
    {
        GetComponentInChildren<UIItemPickupController>().OnItemPickedUp();
        m_OtherInteractor.SetActive(true);
        Destroy(gameObject);
    }
}

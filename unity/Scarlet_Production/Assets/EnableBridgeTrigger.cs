using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBridgeTrigger : Interactor
{
    public GameObject m_OtherInteractor;

    public override void Interact()
    {
        m_OtherInteractor.SetActive(true);
        Destroy(gameObject);
    }
}

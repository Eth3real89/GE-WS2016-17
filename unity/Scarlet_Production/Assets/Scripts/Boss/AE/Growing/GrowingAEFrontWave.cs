using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingAEFrontWave : MonoBehaviour {

    public FrontWaveCallback m_Callback;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerManager>() != null)
        {
            if (m_Callback != null) m_Callback.NotifyAboutRangeFront(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerManager>() != null)
        {
            if (m_Callback != null) m_Callback.NotifyAboutRangeFront(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponentInParent<PlayerManager>() != null)
        {
            if (m_Callback != null) m_Callback.NotifyAboutRangeFront(true);
        }
    }

    public interface FrontWaveCallback
    {
        void NotifyAboutRangeFront(bool isInFront);
    }
}

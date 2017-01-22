using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingAEBackWave : MonoBehaviour {

    public BackWaveCallback m_Callback;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerManager>() != null)
        {
            if (m_Callback != null) m_Callback.NotifyAboutRangeBack(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerManager>() != null)
        {
            if (m_Callback != null) m_Callback.NotifyAboutRangeBack(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponentInParent<PlayerManager>() != null)
        {
            if (m_Callback != null) m_Callback.NotifyAboutRangeBack(true);
        }
    }

    public interface BackWaveCallback
    {
        void NotifyAboutRangeBack(bool isInBack);
    }

}

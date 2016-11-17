using UnityEngine;
using System.Collections;

public class BossDamageTrigger : MonoBehaviour {

    public TriggerCallback m_Callback;


    void OnTriggerEnter(Collider other)
    {
        if (m_Callback != null)
        {
            m_Callback.OnTriggerEnter(other);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (m_Callback != null)
        {
            m_Callback.OnTriggerStay(other);
        }
    }

    void OnTriggerLeave(Collider other)
    {
        if (m_Callback != null)
        {
            m_Callback.OnTriggerLeave(other);
        }
    }
}

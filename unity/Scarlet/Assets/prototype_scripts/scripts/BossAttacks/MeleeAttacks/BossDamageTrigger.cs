using UnityEngine;
using System.Collections;

public class BossDamageTrigger : MonoBehaviour {

    public TriggerCallback m_Callback;

    private AudioSource m_AudioSource;

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

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

    public void PlayHitSound()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.Play();
        }
    }
}

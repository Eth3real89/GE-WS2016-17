using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class GroundControl : MonoBehaviour
{
    [System.Serializable]
    public class ChangeGroundedEvent : UnityEvent<string> { }

    public float m_GroundCheckThreshold;

    public ChangeGroundedEvent m_HitGroundEvent;
    public ChangeGroundedEvent m_LoseGroundEvent;
    private bool m_IsGrounded;
    private string m_GroundTag;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, m_GroundCheckThreshold))
        {
            if (!m_IsGrounded)
            {
                m_IsGrounded = true;
                m_GroundTag = hit.collider.tag;
                m_HitGroundEvent.Invoke(m_GroundTag);
            }
        }
        else
        {
            if (m_IsGrounded)
            {
                m_IsGrounded = false;
                m_LoseGroundEvent.Invoke(null);
            }
        }
    }
}

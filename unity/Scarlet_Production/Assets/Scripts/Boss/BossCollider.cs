using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollider : MonoBehaviour {

    public Rigidbody m_ScarletBody;
    public bool m_Active = false;

    public DamageCollisionHandler m_Handler;

    void OnCollisionEnter(Collision collision)
    {
        OnCollision(collision, true);
    }

    private void OnCollisionStay(Collision collision)
    {
        OnCollision(collision, false);
    }

    private void OnCollision(Collision collision, bool initialCollision)
    {
        if (m_Active && m_Handler != null)
        {
            m_Handler.HandleCollision(collision.collider, initialCollision);
        }

        if (m_Active && m_Handler != null && collision.gameObject.GetComponent<Rigidbody>() == m_ScarletBody)
        {
            m_Handler.HandleScarletCollision(collision.collider);
        }
    }
}

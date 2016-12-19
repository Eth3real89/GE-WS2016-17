using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollider : MonoBehaviour {

    public Rigidbody m_ScarletBody;
    public bool m_Active = false;

    public DefaultCollisionHandler m_Handler;

    void OnCollisionEnter(Collision collision)
    {
        if (m_Active && m_Handler != null && collision.gameObject.GetComponent<Rigidbody>() == m_ScarletBody)
        {
            m_Handler.HandleCollision(collision.collider);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (m_Active && m_Handler != null && collision.gameObject.GetComponent<Rigidbody>() == m_ScarletBody)
        {
            m_Handler.HandleCollision(collision.collider);
        }
    }
}

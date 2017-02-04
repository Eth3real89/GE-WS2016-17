using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToGround : MonoBehaviour
{
    Rigidbody m_Rigidbody;

    private void Start()
    {
        m_Rigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0)
            {
                m_Rigidbody.useGravity = false;
            }
            else
            {
                m_Rigidbody.useGravity = true;
                if (m_Rigidbody.velocity.y > 0)
                {
                    m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);
                }
            }
        }
    }
}

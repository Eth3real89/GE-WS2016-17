using System;
using UnityEngine;

public class VampireFollow : MonoBehaviour
{
    private GameObject m_Player;
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private float m_Offset = 0.5f;

    public bool m_Active;

    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!m_Active)
            return;
        float moveSpeed;

        Vector3 selfPos = transform.position;
        Vector3 targetLocation = GetTargetLocation();
        transform.LookAt(targetLocation);

        if (Vector3.Distance(targetLocation, selfPos) >= 0.5f)
            moveSpeed = 1.1f;
        else if (Vector3.Distance(targetLocation, selfPos) < 0.2f)
            moveSpeed = 0;
        else
            moveSpeed = 0.8f;

        if (moveSpeed > 0)
        {
            m_Rigidbody.velocity = transform.forward * moveSpeed;
            m_Animator.SetFloat("Speed", moveSpeed);
        }
        else
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Animator.SetFloat("Speed", 0);
        }
    }

    private Vector3 GetTargetLocation()
    {
        float offsetX, offsetZ;
        Vector3 playerPos = m_Player.transform.position;
        playerPos.y = transform.position.y;
        offsetZ = m_Offset;

        if (m_Player.transform.forward.x >= 0)
        {
            offsetX = -m_Offset;
        }
        else
        {
            offsetX = m_Offset;
        }

        RaycastHit hit;
        Vector3 targetPos = new Vector3(playerPos.x + offsetX, playerPos.y, playerPos.z + offsetZ);
        float rayLength = Vector3.Distance(transform.position, targetPos);
        if (Physics.Raycast(transform.position, targetPos - transform.position, out hit, rayLength))
        {
            targetPos.z -= m_Offset * 2;
        }
        return targetPos;
    }
}

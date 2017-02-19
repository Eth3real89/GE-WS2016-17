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

    private Rigidbody m_RigidBody;
    private Animator m_Animator;
    private float m_FloatingThreshold = 0.1f;
    private bool m_IsGrounded;
    private string m_GroundTag;

    private void Start()
    {
        InvokeRepeating("CheckIsStuckFalling", 1.0f, 0.5f);
        m_RigidBody = transform.parent.GetComponent<Rigidbody>();
        m_Animator = transform.parent.GetComponent<Animator>();
    }

    void Update()
    {
        if (LevelManager.Instance.m_ControlMode == LevelManager.ControlMode.Combat)
            return;

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

    // Sometimes when Scarlet is standing on an edge, the Raycast misses the floor,
    // which leads to Scarlet being stuck in place in falling state.
    // To avoid this, a force is added to scarlet when she is not grounded and also
    // has no velocity (-> stuck in place), to push her out of her misery
    private void CheckIsStuckFalling()
    {
        if (m_IsGrounded)
            return;
        if (m_Animator.GetBool("IsFalling") && m_RigidBody.velocity.sqrMagnitude <= m_FloatingThreshold)
            m_RigidBody.AddForce(new Vector3(1, 0, 1) * 100, ForceMode.Impulse);
    }
}

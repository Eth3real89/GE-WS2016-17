using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The Move Command is slightly different from the other commands since it can be triggered permanently
 * and does not disable any of the other commands.
 * It moves scarlet around (duh) on the x-z-plane and makes her look in the right direction.
 */
public class PlayerMoveCommand : PlayerCommand
{
    // @todo:
    // these stats should probably not be stored in the command but in a seperate stats-class
    // (also applies to healing amount/charges, etc.)
    public float m_RunSpeedCombat;
    public float m_RunSpeedExploration;
    public float m_WalkSpeed;

    public float m_CurrentSpeed;

    public float m_RaycastRange;
    public Transform m_RaycastAnchor;

    private Rigidbody m_ScarletBody;
    private int m_LayerMask;

    public override void InitTrigger()
    {
        m_LayerMask = ~(1 << 12);
        m_CommandName = "Move";
        m_Trigger = new MoveTrigger(this);

        m_ScarletBody = m_Scarlet.GetComponent<Rigidbody>();
    }

    // unused for move as the values (= how much movement in each direction) actually matter
    public override void TriggerCommand()
    {
    }

    // little hack to control the Player via script
    public void TriggerManually(Vector3 direction)
    {
        DoRotate(direction.x, direction.z);
        DoMove(direction.x, direction.z);
    }

    public void DoMove(float horizontal, float vertical)
    {
        print(m_CurrentSpeed);
        float yBefore = m_ScarletBody.velocity.y;

        Vector3 movement = new Vector3(horizontal, 0, vertical);

        if (IsRunningIntoWall())
            movement = Vector3.zero;

        if (movement.magnitude > 1)
            movement.Normalize();

        movement *= m_CurrentSpeed;
        movement.y = yBefore;

        m_ScarletBody.velocity = movement;

        m_Animator.SetFloat("Speed", movement.magnitude);
    }

    public void StopMoving()
    {
        DoMove(0, 0);
        m_Animator.SetFloat("Speed", 0f);
    }

    private void DoRotate(float horizontal, float vertical)
    {
        if (Mathf.Abs(horizontal) <= 0.05f && Mathf.Abs(vertical) <= 0.05f) return;

        float angle = Mathf.Atan2(horizontal, vertical);

        Quaternion rotation = Quaternion.Euler(0f, Mathf.Rad2Deg * angle, 0f);
        m_ScarletBody.MoveRotation(rotation);
    }

    private bool IsRunningIntoWall()
    {
        return Physics.Raycast(m_RaycastAnchor.position, transform.forward, m_RaycastRange, m_LayerMask);
    }

    // moving has no delay, cannot be cancelled.
    public override void CancelDelay()
    {
    }

    private class MoveTrigger : CommandTrigger
    {
        new PlayerMoveCommand m_Command;

        public MoveTrigger(PlayerMoveCommand command) : base(command)
        {
            m_Command = command;
        }

        public override void Update()
        {
            if (!m_Command.m_Active || !m_Command.IsCommandAvailable())
                return;

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            m_Command.DoMove(horizontal, vertical);
            m_Command.DoRotate(horizontal, vertical);
        }
    }
}

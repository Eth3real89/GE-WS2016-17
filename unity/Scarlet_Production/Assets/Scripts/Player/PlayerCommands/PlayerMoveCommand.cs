using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The Move Command is slightly different from the other commands since it can be triggered permanently
 * and does not disable any of the other commands.
 * It moves scarlet around (duh) on the x-z-plane and makes her look in the right direction.
 */
public class PlayerMoveCommand : PlayerCommand {

    public float m_RunSpeed = 8f;

    private Rigidbody m_ScarletBody;

    public override void InitTrigger()
    {
        m_CommandName = "Move";
        m_Trigger = new MoveTrigger(this);

        m_ScarletBody = m_Scarlet.GetComponent<Rigidbody>();
    }

    // unused for move as the values (= how much movement in each direction) actually matter
    public override void TriggerCommand()
    {
    }

    public void DoMove(float horizontal, float vertical)
    {        
        Vector3 movement = new Vector3(horizontal, 0, vertical);

        if (movement.magnitude > 1)
            movement.Normalize();

        movement *= m_RunSpeed;

        m_ScarletBody.velocity = movement;

        m_Animator.SetFloat("Speed", movement.magnitude);
    }

    private void DoRotate(float horizontal, float vertical)
    {
        if (Mathf.Abs(horizontal) <= 0.05f && Mathf.Abs(vertical) <= 0.05f) return;

        float angle = Mathf.Atan2(horizontal, vertical);

        Quaternion rotation = Quaternion.Euler(0f, Mathf.Rad2Deg * angle, 0f);
        m_ScarletBody.MoveRotation(rotation);
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
            if (!m_Command.m_Active)
                return;

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            m_Command.DoMove(horizontal, vertical);
            m_Command.DoRotate(horizontal, vertical);
        }
    }
}

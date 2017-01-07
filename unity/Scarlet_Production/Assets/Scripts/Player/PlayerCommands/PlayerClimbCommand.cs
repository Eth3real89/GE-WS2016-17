using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbCommand : PlayerCommand
{
    public float m_ClimbSpeed = 1f;

    private Rigidbody m_ScarletBody;

    private void Start()
    {
        m_CommandName = "Dash";
    }

    public override void InitTrigger()
    {
        m_CommandName = "Dash";
        m_Trigger = new ClimbTrigger(this);
        m_ScarletBody = m_Scarlet.GetComponent<Rigidbody>();
    }

    public override void TriggerCommand()
    {
    }

    private void Climb(float horizontal, float vertical)
    {
        Vector3 movement = new Vector3(horizontal, vertical, 0);

        if (movement.magnitude > 1)
            movement.Normalize();

        m_ScarletBody.velocity = movement;
        m_Animator.SetFloat("ClimbingSpeed", movement.magnitude);
    }

    public override void CancelDelay()
    {
    }

    private class ClimbTrigger : CommandTrigger
    {
        new PlayerClimbCommand m_Command;

        public ClimbTrigger(PlayerClimbCommand command) : base(command)
        {
            m_Command = command;
        }

        public override void Update()
        {
            if (!m_Command.m_Active || !m_Command.IsCommandAvailable())
                return;

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            m_Command.Climb(horizontal, vertical);
        }
    }
}

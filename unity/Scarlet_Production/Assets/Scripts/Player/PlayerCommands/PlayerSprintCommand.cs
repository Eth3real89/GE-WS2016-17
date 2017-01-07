using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprintCommand : PlayerCommand
{
    public PlayerMoveCommand m_MoveCommand;

    private void Start()
    {
        m_MoveCommand = GetComponent<PlayerMoveCommand>();
        m_CommandName = "Dash";
    }

    public override void InitTrigger()
    {
        m_CommandName = "Dash";
        m_Trigger = new SprintTrigger(this);
    }

    // sprint needs custom trigger for key release
    public override void TriggerCommand()
    {
    }

    // sprint cannot be cancelled.
    public override void CancelDelay()
    {
    }

    private class SprintTrigger : CommandTrigger
    {
        new PlayerSprintCommand m_Command;

        public SprintTrigger(PlayerSprintCommand command) : base(command)
        {
            m_Command = command;
        }

        public override void Update()
        {
            if (!m_Command.m_Active || !m_Command.IsCommandAvailable())
                return;

            if (Input.GetAxis(m_Command.m_CommandName) > 0)
            {
                m_Command.m_MoveCommand.m_CurrentSpeed = m_Command.m_MoveCommand.m_RunSpeed;
            }
            else
            {
                m_Command.m_MoveCommand.m_CurrentSpeed = m_Command.m_MoveCommand.m_WalkSpeed;
            }
        }
    }
}

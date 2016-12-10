using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveCommand : PlayerCommand {

    public override void InitTrigger()
    {
        m_Trigger = new MoveTrigger(this);
    }

    // unused for move as the values (= how much movement in each direction) actually matter
    public override void TriggerCommand()
    {
    }

    public void DoMove(float horizontal, float vertical)
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

            m_Command.DoMove(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }
}

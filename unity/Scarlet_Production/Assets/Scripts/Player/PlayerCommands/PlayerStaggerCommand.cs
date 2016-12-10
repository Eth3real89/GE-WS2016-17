using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStaggerCommand : PlayerCommand {

    private void Start()
    {
        m_Trigger = new StaggerTrigger(this);
    }

    public override void InitTrigger()
    {
    }

    public override void TriggerCommand()
    {
        m_Callback.OnCommandStart(m_CommandName, this);
        DoStagger();
    }

    private void DoStagger()
    {

    }

    // Stagger trigger remains empty!! stagger is not triggered by the player.
    private class StaggerTrigger : CommandTrigger
    {
        public StaggerTrigger(PlayerCommand command) : base(command)
        {
        }

        public override void Update()
        {
        }
    }
}

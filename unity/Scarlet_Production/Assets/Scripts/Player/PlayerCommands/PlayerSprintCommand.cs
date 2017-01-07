using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprintCommand : PlayerCommand
{
    private void Start()
    {
        m_CommandName = "Dash";
    }

    public override void InitTrigger()
    {
        m_CommandName = "Dash";
        m_Trigger = new ConstantAxisTrigger(this, m_CommandName);
    }

    public override void TriggerCommand()
    {
        Debug.Log("sprint dat shit");
    }

    // dash cannot be cancelled.
    public override void CancelDelay()
    {
    }
}

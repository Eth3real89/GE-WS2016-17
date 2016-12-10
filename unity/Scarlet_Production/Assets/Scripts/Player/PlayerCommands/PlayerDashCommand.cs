using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashCommand : PlayerCommand {

    private void Start()
    {
        m_CommandName = "Dash";
    }

    public override void InitTrigger()
    {
        m_Trigger = new DefaultAxisTrigger(this, m_CommandName);
    }

    public override void TriggerCommand()
    {
        DoDash();
    }
    
    private void DoDash()
    {
        m_Callback.OnCommandStart(m_CommandName, this);
        print("Dashing!");
    }

}

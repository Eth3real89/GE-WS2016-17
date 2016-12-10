using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryCommand : PlayerCommand {

    private void Start()
    {
        m_CommandName = "Parry";
    }

    public override void InitTrigger()
    {
        m_Trigger = new DefaultAxisTrigger(this, m_CommandName);
    }

    public override void TriggerCommand()
    {
        DoParry();
    }

    private void DoParry()
    {
        m_Callback.OnCommandStart(m_CommandName, this);
        print("Parrying!");
    }

}

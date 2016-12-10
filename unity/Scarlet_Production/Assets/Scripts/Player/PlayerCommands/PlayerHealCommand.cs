using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealCommand : PlayerCommand {

    private void Start()
    {
        m_CommandName = "Heal";
    }

    public override void InitTrigger()
    {
        m_Trigger = new DefaultAxisTrigger(this, m_CommandName);
    }

    public override void TriggerCommand()
    {
        DoHeal();
    }

    private void DoHeal()
    {
        print("Healing!");
        m_Callback.OnCommandStart(m_CommandName, this);
    }
}

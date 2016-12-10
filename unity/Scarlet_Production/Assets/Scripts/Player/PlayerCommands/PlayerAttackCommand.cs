using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCommand : PlayerCommand {

    // Use this for initialization
    void Start () {
        m_CommandName = "Attack";
	}

    public override void InitTrigger()
    {
        m_Trigger = new DefaultAxisTrigger(this, m_CommandName);
    }

    public override void TriggerCommand()
    {
        DoAttack();
    }

    private void DoAttack()
    {
        m_Callback.OnCommandStart(m_CommandName, this);
        print("Attacking!!");
    }
}

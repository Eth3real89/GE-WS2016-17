using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCommand : PlayerCommand {

    private Rigidbody m_ScarletBody;

    // Use this for initialization
    void Start () {
        m_CommandName = "Attack";
	}

    public override void InitTrigger()
    {
        m_Trigger = new DefaultAxisTrigger(this, m_CommandName);
        m_ScarletBody = m_Scarlet.GetComponent<Rigidbody>();
    }

    public override void TriggerCommand()
    {
        DoAttack();
    }

    private void DoAttack()
    {
        m_Callback.OnCommandStart(m_CommandName, this);
        m_ScarletBody.velocity = new Vector3(0, 0, 0);
        print("Attacking!!");
    }
}

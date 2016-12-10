using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashCommand : PlayerCommand {

    private Rigidbody m_ScarletBody;

    private void Start()
    {
        m_CommandName = "Dash";
    }

    public override void InitTrigger()
    {
        m_Trigger = new DefaultAxisTrigger(this, m_CommandName);
        m_ScarletBody = m_Scarlet.GetComponent<Rigidbody>();
    }

    public override void TriggerCommand()
    {
        DoDash();
    }
    
    private void DoDash()
    {
        m_Callback.OnCommandStart(m_CommandName, this);
        m_ScarletBody.velocity = new Vector3(0, 0, 0);
        print("Dashing!");
    }

}

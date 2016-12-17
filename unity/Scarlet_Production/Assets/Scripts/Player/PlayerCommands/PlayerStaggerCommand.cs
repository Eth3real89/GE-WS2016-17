using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStaggerCommand : PlayerCommand {

    private Rigidbody m_ScarletBody;

    public float m_StaggerTime;

    private void Start()
    {
        m_CommandName = "Stagger";
    }

    public override void InitTrigger()
    {
        m_CommandName = "Stagger";
        m_Trigger = new StaggerTrigger(this);
        m_ScarletBody = m_Scarlet.GetComponent<Rigidbody>();
    }

    public override void TriggerCommand()
    {
        m_Callback.OnCommandStart(m_CommandName, this);
        DoStagger();

        StartCoroutine(EndStaggerAfter(m_StaggerTime));
    }

    private void DoStagger()
    {
        m_ScarletBody.velocity = new Vector3(0, 0, 0);
        m_Animator.SetTrigger("StaggerTrigger");
    }

    private IEnumerator EndStaggerAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        m_Callback.OnCommandEnd(m_CommandName, this);
    }

    public override void CancelDelay()
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

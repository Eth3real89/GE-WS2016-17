using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStaggerCommand : PlayerCommand {

    protected static PlayerStaggerCommand s_Instance;

    public static void StaggerScarlet(bool throwToGround, Transform awayFrom)
    {
        print("Stagger Scarlet!");
        s_Instance.TriggerCommand();
    }

    private Rigidbody m_ScarletBody;

    public float m_StaggerTime;
    public float m_MajorStaggerTime;

    private void Start()
    {
        if (s_Instance == null)
            s_Instance = this;

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

    public void TriggerMajorStagger()
    {
        m_Callback.OnCommandStart(m_CommandName, this);
        DoStagger(true);

        StartCoroutine(EndStaggerAfter(m_MajorStaggerTime));
    }

    private void DoStagger(bool major = false)
    {
        m_ScarletBody.velocity = new Vector3(0, 0, 0);
        m_Animator.SetTrigger(major? "MajorStaggerTrigger" : "StaggerTrigger");
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

    /*public void OnDamageTaken(Damage dmg)
    {
        if (StaggeringAttack(dmg))
        {
            m_ScarletBody.AddForce((dmg.m_Owner.transform.forward + new Vector3(0, 1, 0)) * m_ScarletBody.mass, ForceMode.Impulse);
        }
    }*/


}

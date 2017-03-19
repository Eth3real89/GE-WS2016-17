using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStaggerCommand : PlayerCommand {

    protected static PlayerStaggerCommand s_Instance;

    public static void StaggerScarlet(bool throwToGround, Vector3 forceValues = new Vector3(), float force = 0f)
    {
        print("Stagger Scarlet!");

        if (throwToGround)
        {
            s_Instance.TriggerMajorStagger(forceValues, force);
        }
        else
        {
            s_Instance.TriggerCommand();
        }
    }

    public static void StaggerScarletAwayFrom(Vector3 origin = new Vector3(), float force = 0f, bool removeY = false)
    {
        print("Stagger Scarlet!");

        Vector3 lookRot = origin - s_Instance.m_ScarletBody.transform.position;
        lookRot.y = 0;
        s_Instance.m_ScarletBody.transform.rotation = Quaternion.Euler(lookRot);

        if (removeY)
            origin.y = 0;

        Vector3 pos = s_Instance.m_ScarletBody.transform.position - origin;
        pos = pos.normalized;
        s_Instance.TriggerMajorStagger(pos, force);
    }

    public static Vector3 ScarletPosition()
    {
        return s_Instance.m_ScarletBody.transform.position * 1f;
    }

    private Rigidbody m_ScarletBody;

    public float m_StaggerTime;
    public float m_MajorStaggerTime;

    protected IEnumerator m_StaggerTimer;

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
        if (m_StaggerTimer != null)
            return;

        m_Callback.OnCommandStart(m_CommandName, this);
        DoStagger();

        m_StaggerTimer = EndStaggerAfter(m_StaggerTime);
        StartCoroutine(m_StaggerTimer);
    }

    public void TriggerMajorStagger(Vector3 forceValues, float force)
    {
        if (m_StaggerTimer != null)
            return;

        m_Callback.OnCommandStart(m_CommandName, this);
        DoStagger(true);

        m_ScarletBody.AddForce(force * forceValues * m_ScarletBody.mass, ForceMode.Impulse);

        m_StaggerTimer = EndStaggerAfter(m_MajorStaggerTime);
        StartCoroutine(m_StaggerTimer);
    }

    private void DoStagger(bool major = false)
    {
        ScarletVOPlayer.Instance.PlayStaggerSound();

        m_ScarletBody.velocity = new Vector3(0, 0, 0);
        m_Animator.SetTrigger(major? "MajorStaggerTrigger" : "StaggerTrigger");
    }

    private IEnumerator EndStaggerAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        m_Callback.OnCommandEnd(m_CommandName, this);

        m_StaggerTimer = null;
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

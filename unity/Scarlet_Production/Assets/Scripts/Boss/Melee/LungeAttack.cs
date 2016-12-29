using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungeAttack : BossAttack, BossJumpCommand.JumpCallback, DamageCollisionHandler {

    public LungeTrigger m_LungeTrigger;
    public BossCollider m_BossCollider;
    public GameObject m_Scarlet;

    public BossTurnCommand m_TurnCommand;
    public BossJumpCommand m_JumpCommand;

    public GameObject m_HitSignal;

    public float m_TrackSpeed = 7f;

    public float m_TrackTime = 4f;
    public float m_HesitateTime = 0.1f;
    public float m_TimeAfterLand = 0.5f;

    private enum State {None, Aim, Jump, Land};
    private State m_State = State.None;

    private IEnumerator m_StateTimer;

    private bool m_ScarletInTargetArea;
    private DefaultCollisionHandler m_CollisionHandler;

    public override void StartAttack()
    {
        base.StartAttack();

        m_LungeTrigger.transform.position = new Vector3(m_Scarlet.transform.position.x, m_LungeTrigger.transform.position.y, m_Scarlet.transform.position.z);
        m_LungeTrigger.GetComponent<Renderer>().enabled = true;
        m_LungeTrigger.m_Active = false;
        m_BossCollider.m_Active = false;
        m_ScarletInTargetArea = false;

        m_Animator.SetTrigger("CrouchTrigger");

        m_StateTimer = Aim();
        StartCoroutine(m_StateTimer);

        m_Callback.OnAttackStart(this);

        m_CollisionHandler = new DefaultCollisionHandler(m_LungeTrigger);
    }

    private IEnumerator Aim()
    {
        m_State = State.Aim;

        float time = 0;
        while ((time += Time.deltaTime) < m_TrackTime)
        {
            UpdateLungeTarget(Time.deltaTime);
            m_TurnCommand.TurnBossTowards(m_LungeTrigger.gameObject);
            yield return null;
        }

        m_StateTimer = JumpAtTarget();
        StartCoroutine(m_StateTimer);
    }

    private void UpdateLungeTarget(float deltaTime)
    {
        Vector3 posScarlet = new Vector3(m_Scarlet.transform.position.x, m_LungeTrigger.transform.position.y, m_Scarlet.transform.position.z);
        Vector3 posGoal = m_LungeTrigger.transform.position;

        if (Vector3.Distance(posScarlet, posGoal) <= deltaTime * m_TrackSpeed)
        {
            m_LungeTrigger.transform.position = posScarlet;
        }
        else
        {
            m_LungeTrigger.transform.position = Vector3.Lerp(posGoal, posScarlet, deltaTime * m_TrackSpeed);
        }
    }

    private IEnumerator JumpAtTarget()
    {
        yield return new WaitForSeconds(m_HesitateTime);

        m_State = State.Jump;
        m_JumpCommand.JumpAt(m_LungeTrigger.transform, this);

        m_BossCollider.m_Active = true;
        m_BossCollider.m_Handler = m_CollisionHandler;

        m_LungeTrigger.m_Active = true;
        m_LungeTrigger.m_CollisionHandler = this;
    }

    public override void CancelAttack()
    {
        if (m_StateTimer != null)
            StopCoroutine(m_StateTimer);

        m_LungeTrigger.GetComponent<Renderer>().enabled = false;
        m_LungeTrigger.m_Active = false;
        m_BossCollider.m_Active = false;

        if (m_HitSignal != null)
            m_HitSignal.SetActive(false);
    }

    public void OnLand()
    {
        m_State = State.Land;

        m_Animator.SetTrigger("UprightTrigger");

        m_StateTimer = WaitAfterLand();
        StartCoroutine(m_StateTimer);
    }

    private IEnumerator WaitAfterLand()
    {
        yield return new WaitForSeconds(m_TimeAfterLand);
        m_State = State.None;

        m_LungeTrigger.GetComponent<Renderer>().enabled = false;
        m_LungeTrigger.m_Active = false;
        m_BossCollider.m_Active = false;
        m_Callback.OnAttackEnd(this);
    }

    public void OnStopMidAir()
    {
        if (m_HitSignal != null)
            m_HitSignal.SetActive(true);
    }

    public void OnContinueMidAir()
    {
        if (m_HitSignal != null)
            m_HitSignal.SetActive(false);
    }

    public bool StopMidAir()
    {
        return m_ScarletInTargetArea;
    }

    public void HandleScarletCollision(Collider other)
    {
        m_ScarletInTargetArea = true;

        if (m_State == State.Land)
            m_CollisionHandler.HandleScarletCollision(other);
    }

    public void HandleCollision(Collider other, bool initialCollision)
    {
        m_CollisionHandler.HandleCollision(other, initialCollision);
    }

    public void HandleScarletLeave(Collider other)
    {
        m_ScarletInTargetArea = false;

        if (m_State == State.Land)
            m_CollisionHandler.HandleScarletLeave(other);
    }
}

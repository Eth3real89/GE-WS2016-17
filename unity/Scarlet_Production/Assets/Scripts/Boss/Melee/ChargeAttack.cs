using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttack : BossAttack, DamageCollisionHandler, HitInterject {

    private enum State {None, Aim, Run};
    private State m_State;

    public GameObject m_Scarlet;
    public BossTurnCommand m_TurnCommand;
    public BossMoveCommand m_MoveCommand;

    public BossCollider m_BossCollider;

    public GameObject[] m_EnvironmentColliderContainers;
    private List<Collider> m_EnvironmentColliders;

    private IEnumerator m_StateTimer;

    public float m_AimTime = 2f;
    public float m_MaxRunTime = 2f;

    public float m_ChargeSpeed = 7f;

    private float m_RunSpeedBefore;

    public override void StartAttack()
    {
        m_State = State.Aim;
        m_StateTimer = StopAimingAfter(m_AimTime);
        StartCoroutine(m_StateTimer);
        m_Callback.OnAttackStart(this);

        m_RunSpeedBefore = m_MoveCommand.m_Speed;
        m_MoveCommand.m_Speed = m_ChargeSpeed;

        m_BossHittable.RegisterInterject(this);

        ReferenceColliders();
    }

    private void ReferenceColliders()
    {
        m_EnvironmentColliders = new List<Collider>();

        if (m_EnvironmentColliderContainers != null)
        {
            foreach (GameObject obj in m_EnvironmentColliderContainers)
            {
                foreach (Collider coll in obj.GetComponentsInChildren<Collider>())
                {
                    m_EnvironmentColliders.Add(coll);
                }
            }
        }
    }

    void Update()
    {
        if (m_State == State.Aim)
        {
            Aim();
        }
        else if (m_State == State.Run)
        {
            Run();
        }
    }

    private void Aim()
    {
        m_TurnCommand.TurnBossTowards(m_Scarlet);
    }

    private IEnumerator StopAimingAfter(float time)
    {
        yield return new WaitForSeconds(time);
        m_State = State.Run;

        m_BossCollider.m_Handler = this;
        m_BossCollider.m_Active = true;

        m_StateTimer = StopRunnigAfter(m_MaxRunTime);
        StartCoroutine(m_StateTimer);
    }

    private void Run()
    {
        m_MoveCommand.DoMove(m_Boss.transform.forward.x, m_Boss.transform.forward.z);
    }

    private IEnumerator StopRunnigAfter(float time)
    {
        yield return new WaitForSeconds(time);
        m_State = State.None;
        m_MoveCommand.StopMoving();

        m_Callback.OnAttackInterrupted(this);
        m_MoveCommand.m_Speed = m_RunSpeedBefore;
    }

    public override void CancelAttack()
    {
        m_State = State.None;

        if (m_StateTimer != null)
            StopCoroutine(m_StateTimer);

        m_MoveCommand.m_Speed = m_RunSpeedBefore;
    }

    public void HandleCollision(Collider other, bool initialCollision)
    {
        if (initialCollision && m_State == State.Run)
        {
            if (m_EnvironmentColliders.IndexOf(other) >= 0)
            {
                if (m_StateTimer != null)
                    StopCoroutine(m_StateTimer);

                m_State = State.None;
                m_MoveCommand.StopMoving();

                m_Callback.OnAttackInterrupted(this);
                m_MoveCommand.m_Speed = m_RunSpeedBefore;
            }
        }
    }

    public void HandleScarletCollision(Collider other)
    {
        Hittable hittable = other.GetComponent<Hittable>();
        if (hittable != null)
        {
            hittable.Hit(new ChargeDamage());

            m_BossCollider.m_Active = false;
        }
    }

    public bool OnHit(Damage dmg)
    {
        if (m_State == State.Aim)
        {
            return false;
        }
        else if (m_State == State.Run)
        {
            dmg.OnBlockDamage();
            return true;
        }

        return false;
    }

    private class ChargeDamage : Damage
    {
        public override bool Blockable()
        {
            return false;
        }

        public override float DamageAmount()
        {
            return 40;
        }
    }
}

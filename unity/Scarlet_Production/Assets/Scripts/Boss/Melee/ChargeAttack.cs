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
    public BossStaggerCommand m_StaggerCommand;

    public BossCollider m_BossCollider;
    public BossMeleeDamage m_DamageTrigger;

    public GameObject[] m_EnvironmentColliderContainers;
    private List<Collider> m_EnvironmentColliders;

    private IEnumerator m_StateTimer;

    public float m_AimTime = 2f;
    public float m_MaxRunTime = 2f;

    public float m_ChargeSpeed = 7f;

    public float m_StaggerTimeOnWallHit = 1.5f;
    private IEnumerator m_StaggerTimer;

    private float m_RunSpeedBefore;

    private bool m_CarryingScarlet = false;

    public override void StartAttack()
    {
        base.StartAttack();

        m_State = State.Aim;
        m_StateTimer = StopAimingAfter(m_AimTime);
        StartCoroutine(m_StateTimer);

        m_RunSpeedBefore = m_MoveCommand.m_Speed;
        m_MoveCommand.m_Speed = m_ChargeSpeed;

        m_CarryingScarlet = false;

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

            if (m_CarryingScarlet)
            {
                MoveScarlet();
            }
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

        m_DamageTrigger.m_CollisionHandler = new ChargeHitWallWithScarletCallback(this);
        m_DamageTrigger.m_Active = true;

        m_StateTimer = StopRunnigAfter(m_MaxRunTime);
        StartCoroutine(m_StateTimer);
    }

    private void Run()
    {
        m_MoveCommand.DoMove(m_Boss.transform.forward.x, m_Boss.transform.forward.z);
    }

    private void MoveScarlet()
    {
        Rigidbody scarletBody = m_Scarlet.GetComponent<Rigidbody>();
        Rigidbody bossBody = m_BossCollider.GetComponentInChildren<Rigidbody>();
        if (scarletBody != null && bossBody != null)
        {
            scarletBody.velocity = bossBody.velocity;
        }
    }

    private IEnumerator StopRunnigAfter(float time)
    {
        yield return new WaitForSeconds(time);
        m_State = State.None;
        m_MoveCommand.StopMoving();

        m_Callback.OnAttackEnd(this);
        m_MoveCommand.m_Speed = m_RunSpeedBefore;

        m_DamageTrigger.m_Active = false;
        m_BossCollider.m_Active = false;

        if (m_CarryingScarlet)
        {
            EnableScarletControls();
            MoveScarlet(); // in this case: break!
        }
        m_CarryingScarlet = false;
    }

    public override void CancelAttack()
    {
        m_State = State.None;

        if (m_StateTimer != null)
            StopCoroutine(m_StateTimer);

        if (m_StaggerTimer != null)
            StopCoroutine(m_StaggerTimer);

        m_MoveCommand.m_Speed = m_RunSpeedBefore;
        if (m_CarryingScarlet)
            EnableScarletControls();

        m_DamageTrigger.m_Active = false;
        m_BossCollider.m_Active = false;

        m_CarryingScarlet = false;
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
                m_MoveCommand.m_Speed = m_RunSpeedBefore;

                if (m_CarryingScarlet)
                {
                    EnableScarletControls();
                    m_CarryingScarlet = false;
                    DealDamageToScarlet(m_Scarlet, new ChargeHitWallDamage());
                }

                if (m_StaggerTimeOnWallHit > 0)
                {
                    m_StaggerCommand.DoStagger();

                    m_StaggerTimer = WaitWhileStaggering();
                    StartCoroutine(m_StaggerTimer);
                }
            }
        }
    }

    private IEnumerator WaitWhileStaggering()
    {
        yield return new WaitForSeconds(m_StaggerTimeOnWallHit);
        m_Callback.OnAttackEnd(this);
    }

    public void HandleScarletCollision(Collider other)
    {
        DealDamageToScarlet(m_Scarlet, new ChargePickUpDamage());
        DisableScarletControls();
    }

    private void DealDamageToScarlet(GameObject other, Damage damage)
    {
        Hittable hittable = other.GetComponent<Hittable>();
        if (hittable != null)
        {
            hittable.Hit(damage);
            m_BossCollider.m_Active = false;

            m_CarryingScarlet = true;
        }
    }

    private void DisableScarletControls()
    {
        PlayerControls controls = m_Scarlet.GetComponentInChildren<PlayerControls>();
        if (controls != null)
        {
            controls.DisableAllCommands();
        }
    }

    private void EnableScarletControls()
    {
        PlayerControls controls = m_Scarlet.GetComponentInChildren<PlayerControls>();
        if (controls != null)
        {
            controls.EnableAllCommands();
        }
    }

    public void HandleScarletLeave(Collider other)
    {
    }

    private class ChargePickUpDamage : Damage
    {
        public override BlockableType Blockable()
        {
            return BlockableType.None;
        }

        public override float DamageAmount()
        {
            return 15;
        }
    }

    private class ChargeHitWallDamage : Damage
    {
        public override BlockableType Blockable()
        {
            return BlockableType.None;
        }

        public override float DamageAmount()
        {
            return 60;
        }
    }

    private class ChargeHitWallWithScarletCallback : DamageCollisionHandler
    {

        private ChargeAttack m_Attack;

        public ChargeHitWallWithScarletCallback(ChargeAttack attack)
        {
            m_Attack = attack;
        }

        public void HandleCollision(Collider other, bool initialCollision)
        {
            if (m_Attack.m_CarryingScarlet)
            {
                m_Attack.HandleCollision(other, initialCollision);
            }
        }

        public void HandleScarletCollision(Collider other)
        {
        }

        public void HandleScarletLeave(Collider other)
        {
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttack : BossAttack, DamageCollisionHandler {

    public static string START_RUNNING_EVENT = "charge_attack_start_run";

    private enum State {None, Aim, Run};
    private State m_State;

    public GameObject m_Scarlet;
    public BossTurnCommand m_TurnCommand;
    public BossMoveCommand m_MoveCommand;
    public BossStaggerCommand m_StaggerCommand;

    public BossCollider m_BossCollider;
    public BossMeleeDamage m_DamageTrigger;

    public GameObject[] m_EnvironmentColliderContainers;
    private List<GameObject> m_EnvironmentColliders;

    public Damage m_ChargePickupDamage;
    public Damage m_ChargeHitWallDamage;

    private IEnumerator m_StateTimer;

    public float m_AimTime = 2f;
    public float m_MaxRunTime = 2f;

    public float m_ChargeSpeed = 7f;

    public float m_StaggerTimeOnWallHit = 1.5f;
    private IEnumerator m_StaggerTimer;

    private float m_RunSpeedBefore;

    private bool m_CarryingScarlet = false;

    protected bool m_SpeedSet = false;

    public override void StartAttack()
    {
        base.StartAttack();

        m_State = State.Aim;
        m_StateTimer = StopAimingAfter(m_AimTime);
        StartCoroutine(m_StateTimer);

        m_SpeedSet = true;
        m_RunSpeedBefore = m_MoveCommand.m_Speed;
        m_MoveCommand.m_Speed = m_ChargeSpeed;

        m_CarryingScarlet = false;

        ReferenceColliders();
    }

    private void ReferenceColliders()
    {
        m_EnvironmentColliders = new List<GameObject>();

        if (m_EnvironmentColliderContainers != null)
        {
            foreach (GameObject obj in m_EnvironmentColliderContainers)
            {
                foreach (Collider coll in obj.GetComponentsInChildren<Collider>())
                {
                    m_EnvironmentColliders.Add(coll.gameObject);
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
        m_Animator.SetTrigger("TurnTrigger");

        yield return new WaitForSeconds(time);

        m_Animator.SetTrigger("ChargeStartTrigger");
        yield return new WaitForSeconds(0.5f);

        EventManager.TriggerEvent(START_RUNNING_EVENT);
        m_State = State.Run;

        m_BossCollider.m_Handler = this;
        m_BossCollider.m_Active = true;

        m_DamageTrigger.m_CollisionHandler = new ChargeHitWallWithScarletCallback(this);
        m_DamageTrigger.m_Active = true;

        m_StateTimer = StopRunnigAfter(m_MaxRunTime);
        StartCoroutine(m_StateTimer);
        m_Animator.SetTrigger("ChargeTrigger");
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

        m_DamageTrigger.m_Active = false;
        m_BossCollider.m_Active = false;

        if (m_CarryingScarlet)
        {
            m_Animator.SetTrigger("ChargePinTrigger");
            yield return new WaitForSeconds(0.5f);
            DealDamageToScarlet(m_Scarlet, m_ChargeHitWallDamage);
            yield return new WaitForSeconds(0.5f);

            EnableScarletControls();
            MoveScarlet(); // in this case: break!
        }

        m_MoveCommand.StopMoving();
        m_MoveCommand.m_Speed = m_RunSpeedBefore;
        m_SpeedSet = false;

        m_Callback.OnAttackEnd(this);

        m_CarryingScarlet = false;
    }

    public override void CancelAttack()
    {
        m_State = State.None;

        if (m_StateTimer != null)
            StopCoroutine(m_StateTimer);

        if (m_StaggerTimer != null)
            StopCoroutine(m_StaggerTimer);

        if (m_SpeedSet)
        {
            m_MoveCommand.m_Speed = m_RunSpeedBefore;
        }
        m_SpeedSet = false;

        if (m_CarryingScarlet)
            EnableScarletControls();

        m_DamageTrigger.m_Active = false;
        m_BossCollider.m_Active = false;

        m_CarryingScarlet = false;
    }

    public void HandleCollision(Collider other, bool initialCollision)
    {
        if (m_State == State.Run && initialCollision)
        {
            if (m_EnvironmentColliders.IndexOf(other.gameObject) >= 0)
            {
                if (m_StateTimer != null)
                    StopCoroutine(m_StateTimer);

                m_State = State.None;
                m_MoveCommand.StopMoving();
                m_MoveCommand.m_Speed = m_RunSpeedBefore;

                m_DamageTrigger.m_Active = false;
                m_BossCollider.m_Active = false;

                if (m_CarryingScarlet)
                {
                    EnableScarletControls();
                    m_CarryingScarlet = false;
                    DealDamageToScarlet(m_Scarlet, m_ChargeHitWallDamage);
                }

                if (m_StaggerTimeOnWallHit > 0)
                {
                    m_StaggerCommand.DoStagger("StaggerHardTrigger");

                    m_StaggerTimer = WaitWhileStaggering();
                    StartCoroutine(m_StaggerTimer);
                }
            }
        }
    }

    private IEnumerator WaitWhileStaggering()
    {
        m_MoveCommand.StopMoving();
        yield return new WaitForEndOfFrame();
        m_MoveCommand.StopMoving();

        yield return new WaitForSeconds(m_StaggerTimeOnWallHit);
        m_Callback.OnAttackEnd(this);
    }

    public void HandleScarletCollision(Collider other)
    {
        DealDamageToScarlet(m_Scarlet, m_ChargePickupDamage);
        DisableScarletControls();
    }

    private void DealDamageToScarlet(GameObject other, Damage damage)
    {
        PlayerHittable hittable = other.GetComponent<PlayerHittable>();
        if (hittable != null)
        {
            hittable.MakeVulnerable(true);
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

    private void StopScarletMovement()
    {
        PlayerMoveCommand move = m_Scarlet.GetComponentInChildren<PlayerMoveCommand>();
        if (move != null)
        {
            move.StopMoving();
        }

        Rigidbody scarletBody = m_Scarlet.GetComponent<Rigidbody>();
        if (scarletBody != null )
        {
            scarletBody.velocity = new Vector3(0, 0, 0);
        }
    }

    public void HandleScarletLeave(Collider other)
    {
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
            //if (m_Attack.m_CarryingScarlet)
            //{
                m_Attack.HandleCollision(other, initialCollision);
            // }
        }

        public void HandleScarletCollision(Collider other)
        {
        }

        public void HandleScarletLeave(Collider other)
        {
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseAttack : BossAttack, BossMeleeHitCommand.MeleeHitCallback, DamageCollisionHandler, Damage.DamageCallback {

    public BossMoveCommand m_BossMove;
    public TurnTowardsScarlet m_BossTurn;
    public BossMeleeHitCommand m_BossHit;
    public BossMeleeDamage m_RangeTrigger;

    public GameObject m_Target;

    public Damage.BlockableType m_Blockable = Damage.BlockableType.Parry;

    public float m_MaxChaseTime = 7f;
    private float m_CurrentChaseTime;

    public float m_MaxTurnAngleChaseState = 120f;
    public float m_MaxTurnAngleTurnState = 180f;
    public bool m_AllowTurnWhileAttacking = false;

    public bool m_AllowRunWhileTurning = false;

    private enum AttackState {None, Chase, Turn, Attack};
    private AttackState m_State = AttackState.None;

    private bool m_ScarletInRange = false;

    public float m_DamageAmount = 30f;
    public int m_AttackAnimation = 0;

    public bool m_SkipAttack = false;

    public override void StartAttack()
    {
        base.StartAttack();

        m_State = AttackState.Chase;
        m_BossTurn.m_TurnSpeed = m_MaxTurnAngleTurnState;
        m_RangeTrigger.m_CollisionHandler = this;
        m_RangeTrigger.m_Active = true;
        m_RangeTrigger.m_Callback = this;

        m_BossHit.m_DamageTrigger.m_Amount = this.m_DamageAmount;

        m_ScarletInRange = false;
        m_CurrentChaseTime = 0f;

        CheckRange();
    }

    void Update ()
    {
        if (m_State == AttackState.Chase)
        {
            Chase();
        }
        else if (m_State == AttackState.Turn)
        {
            Turn();

            if (m_AllowRunWhileTurning && m_State != AttackState.Attack)
            {
                Chase();
            }
        }
        else if (m_State == AttackState.Attack && m_AllowTurnWhileAttacking)
        {
            m_BossTurn.DoTurn();
        }
    }

    void Chase()
    {
        m_BossTurn.DoTurn();

        if (Mathf.Abs(m_BossTurn.m_AngleTowardsScarlet) <= 45)
        {
            m_BossMove.DoMove(m_Boss.transform.forward.x, m_Boss.transform.forward.z, m_BossTurn.m_Scarlet.transform.position);
            m_BossTurn.m_TurnSpeed = m_MaxTurnAngleChaseState;
        }
        else
        {
            m_State = AttackState.Turn;
            if (!m_AllowRunWhileTurning)    
                m_BossMove.StopMoving();
            m_BossTurn.m_TurnSpeed = m_MaxTurnAngleTurnState;
        }

        CheckRange();
   }

    void Turn()
    {
        m_BossTurn.DoTurn();

        if (Mathf.Abs(m_BossTurn.m_AngleTowardsScarlet) <= 15)
        {
            m_State = AttackState.Chase;
        }
        
        CheckRange();
    }

    private void CheckRange()
    {
        if (m_ScarletInRange)
        {
            StartHit();
        }
        else
        {
            CheckChaseTime();
        }

    }

    private void CheckChaseTime()
    {
        m_CurrentChaseTime += Time.deltaTime;

        if (m_CurrentChaseTime >= m_MaxChaseTime)
        {
            m_State = AttackState.None;
            CancelAttack();
            m_Callback.OnAttackEndUnsuccessfully(this);
        }
    }

    void StartHit()
    {
        if (m_SkipAttack)
        {
            m_State = AttackState.None;
            m_Callback.OnAttackEnd(this);

            return;
        }

        m_State = AttackState.Attack;
        m_BossHit.DoHit(this, null, m_AttackAnimation);
        m_RangeTrigger.m_Blockable = this.m_Blockable;
        m_BossMove.StopMoving();
    }
    
    public override void CancelAttack()
    {
        m_State = AttackState.None;
        m_BossHit.CancelHit();
    }
    
    public void OnMeleeHitEnd()
    {
        m_Callback.OnAttackEnd(this);
        m_State = AttackState.None;
    }

    public void OnMeleeHitSuccess()
    {
        m_State = AttackState.None;
    }

    public void HandleScarletCollision(Collider other)
    {
        Hittable hittable = other.GetComponentInChildren<Hittable>();
        if (hittable != null && hittable is PlayerHittable)
        {
            m_ScarletInRange = true;
        }
    }

    public void HandleCollision(Collider other, bool initialCollision)
    {
    }

    public void OnParryDamage()
    {
        m_Callback.OnAttackParried(this);
    }

    public void OnBlockDamage()
    {
    }

    public void OnSuccessfulHit()
    {
    }

    public void HandleScarletLeave(Collider other)
    {
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatedMeleeAttack : BossAttack, BossMeleeHitCommand.MeleeHitCallback, Damage.DamageCallback, BossMeleeHitCommand.MeleeHitStepsCallback
{
    public Damage.BlockableType m_Blockable = Damage.BlockableType.Parry;

    public int[] m_HitAnimationIndices = {0};

    public int m_Repetitions = 3;
    private int m_CurrentRepetition;

    public float m_MovementSpeed = 0.5f;

    public BossMoveCommand m_BossMove;
    public BossMeleeHitCommand m_BossHit;
    public BossMeleeDamage m_Damage;

    public TurnTowardsScarlet m_BossTurn;
    public bool m_AllowTurnWhileAttacking = false;

    private IEnumerator m_BetweenHitsTimer;
    public float m_TimeBetweenHits = 0.5f;

    public float m_DamageAmount = 30f;

    private bool m_Attacking = false;

    private void Update()
    {
        if (m_Attacking && m_AllowTurnWhileAttacking)
        {
            m_BossTurn.DoTurn();
        }
    }

    public override void StartAttack()
    {
        base.StartAttack();

        m_CurrentRepetition = 0;
        m_BossHit.DoHit(this, this, m_HitAnimationIndices[m_CurrentRepetition % m_HitAnimationIndices.Length]);
        m_Damage.m_Callback = this;
        m_Damage.m_Blockable = this.m_Blockable;

        m_Damage.m_Amount = this.m_DamageAmount;

        m_Attacking = false;
        m_Damage.m_CollisionHandler = new DefaultCollisionHandler(m_Damage);
    }

    public override void CancelAttack()
    {
        if (m_BetweenHitsTimer != null)
            StopCoroutine(m_BetweenHitsTimer);

        m_BossHit.CancelHit();
        m_Damage.m_Active = false;
        m_Attacking = false;
        m_BossMove.StopMoving();
    }

    public void OnMeleeHitSuccess()
    {
        m_Attacking = false;
    }

    public void OnMeleeHitEnd()
    {
        m_Attacking = false;
        m_BossMove.StopMoving();

        m_CurrentRepetition++;
        if (m_CurrentRepetition >= m_Repetitions)
        {
            m_Callback.OnAttackEnd(this);
        }
        else
        {
            m_BetweenHitsTimer = DoNextHitAfter(m_TimeBetweenHits);
            StartCoroutine(m_BetweenHitsTimer);
        }
    }

    private IEnumerator DoNextHitAfter(float time)
    {
        yield return new WaitForSeconds(time);

        m_BossHit.DoHit(this, this, m_CurrentRepetition % 2);
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

    public void OnMeleeDownswingStart()
    {
        if (m_BossTurn.DistanceToScarlet() >= 1)
            m_BossMove.DoMove(m_Boss.transform.forward.x * m_MovementSpeed, m_Boss.transform.forward.z * m_MovementSpeed);
    }

    public void OnMeleeHalt()
    {
    }

    public void OnMeleeUpswingStart()
    {
        m_Attacking = true;
    }

    public void OnMeleeEnd()
    {
        m_Attacking = false;
    }
}

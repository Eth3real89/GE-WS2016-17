using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatedMeleeAttack : BossAttack, BossMeleeHitCommand.MeleeHitCallback, Damage.DamageCallback, BossMeleeHitCommand.MeleeHitStepsCallback
{
    public enum StaggerLevel { None, ALittle, Hard };
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
    public bool m_TurnBeforeEachAttack = true;
    private float m_TurnSpeedBeforeAttack = 360;
    private float m_PreviousTurnSpeed;

    private IEnumerator m_BetweenHitsTimer;
    public float m_TimeBetweenHits = 0.5f;

    public float m_DamageAmount = 30f;

    private bool m_Attacking = false;
    public StaggerLevel m_StaggerScarlet;

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
        HandleTurnBeforeAttack();

        m_CurrentRepetition = 0;
        m_BossHit.DoHit(this, this, m_HitAnimationIndices[m_CurrentRepetition % m_HitAnimationIndices.Length]);
        m_Damage.m_Callback = this;
        m_Damage.m_Blockable = this.m_Blockable;

        m_Damage.m_Amount = this.m_DamageAmount;

        m_Attacking = false;
        m_Damage.m_CollisionHandler = new DefaultCollisionHandler(m_Damage);
    }

    private void HandleTurnBeforeAttack()
    {
        if (m_TurnBeforeEachAttack)
        {
            StartCoroutine(TurnBeforeAttackParallel());
        }
    }

    private IEnumerator TurnBeforeAttackParallel()
    {
        m_PreviousTurnSpeed = m_BossTurn.m_TurnSpeed;
        m_BossTurn.m_TurnSpeed = m_TurnSpeedBeforeAttack;

        float time = 0;
        while (time < 0.1)
        {
            m_BossTurn.DoTurn();

            time += Time.deltaTime;
            yield return null;
        }

        m_BossTurn.m_TurnSpeed = m_PreviousTurnSpeed;
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
        HandleTurnBeforeAttack();
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
        if (m_StaggerScarlet == StaggerLevel.ALittle)
        {
            PlayerStaggerCommand.StaggerScarlet(false);
        }
        else if (m_StaggerScarlet == StaggerLevel.Hard)
        {
            PlayerStaggerCommand.StaggerScarletAwayFrom(transform.position, 2f);
        }
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

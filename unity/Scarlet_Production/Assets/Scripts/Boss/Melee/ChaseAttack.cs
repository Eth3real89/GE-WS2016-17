using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseAttack : BossAttack {

    public BossMoveCommand m_BossMove;
    public TurnTowardsScarlet m_BossTurn;
    public BossMeleeHitCommand m_BossHit;

    public GameObject m_Target;

    private enum AttackState {None, Chase, Turn, Attack};
    private AttackState m_State = AttackState.None;

    public override void StartAttack()
    {
        m_State = AttackState.Chase;
        m_BossTurn.m_TurnSpeed = 120;
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
        }
	}

    void Chase()
    {
        Vector3 distance = m_Target.transform.position - m_Boss.transform.position;

        if (distance.magnitude >= 1)
        {
            m_BossTurn.DoTurn();

            if (Mathf.Abs(m_BossTurn.m_AngleTowardsScarlet) <= 45)
            {
                m_BossMove.DoMove(distance.x, distance.z);
                m_BossTurn.m_TurnSpeed = 60f;
            }
            else
            {
                m_State = AttackState.Turn;
                m_BossMove.StopMoving();
                m_BossTurn.m_TurnSpeed = 120;
            }
        }
        else
        {
            m_BossMove.StopMoving();
            m_State = AttackState.Attack;
            StartHit();
        }
    }

    void Turn()
    {
        m_BossTurn.DoTurn();

        if (Mathf.Abs(m_BossTurn.m_AngleTowardsScarlet) <= 15)
        {
            m_State = AttackState.Chase;
        }
    }

    void StartHit()
    {
        print("Upswing!");
    }

    void SignalDownSwing()
    {
        print("Downsing incoming!");
    }

    void DownSwing()
    {
        print("Downswing!");
    }

    public override void CancelAttack()
    {
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseAttack : BossAttack {

    public BossMoveCommand m_BossMove;
    public BossMeleeHitCommand m_BossHit;

    public GameObject m_Target;

    private enum AttackState {None, Chase, Attack};
    private AttackState m_State = AttackState.None;

    public override void StartAttack()
    {
        m_State = AttackState.Chase;
    }

    void Update ()
    {
		if (m_State == AttackState.Chase)
        {
            Chase();
        }
	}

    void Chase()
    {
        Vector3 distance = m_Target.transform.position - m_Boss.transform.position;

        if (distance.magnitude >= 1)
        {
            m_BossMove.DoMove(distance.x, distance.z);
        }
        else
        {
            m_BossMove.StopMoving();
            m_State = AttackState.Attack;
            StartHit();
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

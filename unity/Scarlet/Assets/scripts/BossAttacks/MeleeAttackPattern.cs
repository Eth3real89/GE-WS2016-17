using UnityEngine;
using System.Collections;
using System;

public class MeleeAttackPattern : AttackPattern, AttackCallbacks {

	// Use this for initialization
	void Start () {

        base.m_Attacks = new Attack[1];
        base.m_Attacks[0] = new ChaseAttack(this);
        base.m_Attacks[0].m_Callbacks = this;   

        ((ChaseAttack) base.m_Attacks[0]).StartAttack();
    }
	
	// Update is called once per frame
	void Update () {
        if (m_CurrentAttack != null)
        {
            m_CurrentAttack.WhileActive();
        }
    }

    void AttackCallbacks.OnAttackStart(Attack a)
    {
        m_CurrentAttack = a;
    }

    void AttackCallbacks.OnAttackEnd(Attack a)
    {
    }

    void AttackCallbacks.OnAttackParried(Attack a)
    {
        Debug.Log("Attack Parried!");
        m_CurrentAttack = null;
    }

    void AttackCallbacks.OnAttackCancelled(Attack a)
    {
        Debug.Log("Attack Cancelled!");
        m_CurrentAttack = null;
    }
}

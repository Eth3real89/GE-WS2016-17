using UnityEngine;
using System.Collections;
using System;

public class MeleeAttackPattern : AttackPattern, AttackCallbacks {

    private Attack activeAttack;

	// Use this for initialization
	void Start () {

        base.m_Attacks = new Attack[1];
        base.m_Attacks[0] = new ChaseAttack(this);
        base.m_Attacks[0].m_Callbacks = this;   

        ((ChaseAttack) base.m_Attacks[0]).StartAttack();
    }
	
	// Update is called once per frame
	void Update () {
        if (activeAttack != null)
        {
            activeAttack.WhileActive();
        }
    }

    void AttackCallbacks.OnAttackStart(Attack a)
    {
        activeAttack = a;
    }

    void AttackCallbacks.OnAttackEnd(Attack a)
    {
    }
}

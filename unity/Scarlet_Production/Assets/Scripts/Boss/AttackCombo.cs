using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCombo : MonoBehaviour, BossAttack.AttackCallback {

    public enum Interruptability {None, AnyHit, SpecialHit};
    public enum TriggerParry {Never, AnyHit, SpecialHit};

    public GameObject m_Boss;
    public Animator m_Animator;

    public BossAttack[] m_Attacks;

    public Interruptability m_Interruptable;
    public TriggerParry m_TriggerParry;

    /// <summary>
    /// If an attack was interrupted and the player keeps hitting,
    /// can the boss still parry or will it have to take damage for a while?
    /// </summary>
    public bool m_ParryTrumpsInterrupt;

    public float m_TimeAfterCombo;

    public ComboCallback m_Callback;
    private int m_CurrentAttackIndex;
   
	void Start ()
    {
		foreach(BossAttack attack in m_Attacks)
        {
            attack.m_Boss = m_Boss;
            attack.m_Animator = m_Animator;
        }
	}
	
	// Update is called once per frame
	void Update () {
        		
	}

    public void LaunchCombo()
    {
        m_CurrentAttackIndex = 0;
        m_Attacks[m_CurrentAttackIndex].StartAttack();
    }

    public void OnAttackStart(BossAttack attack)
    {
    }

    public void OnAttackEnd(BossAttack attack)
    {
        m_CurrentAttackIndex++;
        if (m_CurrentAttackIndex >= m_Attacks.Length)
        {
            m_Callback.OnComboEnd(this);
        }
        else
        {
            m_Attacks[m_CurrentAttackIndex].StartAttack();
        }
    }

    public void OnAttackInterrupted(BossAttack attack)
    {
    }

    // @todo: cancel / parry behaviour

    public interface ComboCallback
    {
        void OnComboStart(AttackCombo combo);
        void OnComboEnd(AttackCombo combo);

        void OnActivateParry(AttackCombo combo);
        void OnInterruptCombo(AttackCombo combo);
    }

}

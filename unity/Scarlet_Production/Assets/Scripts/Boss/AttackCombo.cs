using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCombo : MonoBehaviour, BossAttack.AttackCallback, HitInterject {

    public enum Interruptability {None, AnyHit, SpecialHit};
    public enum TriggerParry {Never, AnyHit, SpecialHit};

    public GameObject m_Boss;
    public Animator m_Animator;
    public WerewolfHittable m_BossHittable;
    public BossStaggerCommand m_BossStagger;

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
    private BossAttack m_CurrentAttack;
    private int m_CurrentAttackIndex;

    private bool m_BetweenAttacks;
    private IEnumerator m_AttackTimer;
   
	void Start ()
    {
		foreach(BossAttack attack in m_Attacks)
        {
            attack.m_Boss = m_Boss;
            attack.m_Animator = m_Animator;
            attack.m_Callback = this;
        }
	}

    public void LaunchCombo()
    {
        m_CurrentAttackIndex = 0;
        m_BossHittable.RegisterInterject(this);
        m_Attacks[m_CurrentAttackIndex].StartAttack();
    }

    public void OnAttackStart(BossAttack attack)
    {
        m_BetweenAttacks = false;
        m_CurrentAttack = attack;
    }

    public void OnAttackEnd(BossAttack attack)
    {
        m_BetweenAttacks = true;
        m_CurrentAttack = null;
        m_BossHittable.RegisterInterject(this);

        m_CurrentAttackIndex++;
        if (m_CurrentAttackIndex >= m_Attacks.Length)
        {
            m_Callback.OnComboEnd(this);
        }
        else
        {
            m_AttackTimer = StartNextAttackAfter(m_Attacks[m_CurrentAttackIndex - 1].m_TimeAfterAttack);
            StartCoroutine(m_AttackTimer);
        }
    }

    private IEnumerator StartNextAttackAfter(float time)
    {
        yield return new WaitForSeconds(time);
        m_Attacks[m_CurrentAttackIndex].StartAttack();
    }

    public void OnAttackInterrupted(BossAttack attack)
    {
        // @todo might do something more sophisticated such as switch to a different attack?
        // (tbd when there is more than one kind of attack ;) )
        OnAttackEnd(attack);
    }

    public bool OnHit(Damage dmg)
    {
        // @todo: maybe the individual attacks can take care of this as long as they are active?
        // (so the combo behaviour is active unless an attack is active & wants to override it)

        // @todo dmg needs a type? at least player damage...

        if (m_Interruptable == Interruptability.None)
        {
            if (m_TriggerParry == TriggerParry.AnyHit)
            {
                ParryAttack(dmg);
                return true;
            }

            return false;
        }
        else if (m_Interruptable == Interruptability.AnyHit)
        {
            if (m_ParryTrumpsInterrupt)
            {
                if (m_TriggerParry == TriggerParry.AnyHit)
                {
                    ParryAttack(dmg);
                    return true;
                }
                else
                {
                    Interrupt(dmg);
                    return false;
                }
            }
            else
            {
                Interrupt(dmg);
                return false;
            }
        }

        return false;
    }

    private void ParryAttack(Damage dmg)
    {
        // @todo separate animation!
        m_Animator.SetTrigger("MeleeDownswingTrigger");

        if (m_AttackTimer != null)
            StopCoroutine(m_AttackTimer);

        if (m_CurrentAttack != null)
        {
            m_CurrentAttack.CancelAttack();
        }
        dmg.OnParryDamage();

        // @todo more elegant solution
        m_BossHittable.RegisterInterject(null);

        m_Callback.OnActivateParry(this);
    }

    private void Interrupt(Damage dmg)
    {
        if (!m_BetweenAttacks && m_CurrentAttack != null)
        {
            m_CurrentAttack.CancelAttack();
        }
        m_BossStagger.DoStagger();

        m_Callback.OnInterruptCombo(this);
    }

    public interface ComboCallback
    {
        void OnComboStart(AttackCombo combo);
        void OnComboEnd(AttackCombo combo);

        void OnActivateParry(AttackCombo combo);
        void OnInterruptCombo(AttackCombo combo);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCombo : MonoBehaviour, BossAttack.AttackCallback, HitInterject {

    public GameObject m_Boss;
    public Animator m_Animator;
    public WerewolfHittable m_BossHittable;
    public BossStaggerCommand m_BossStagger;

    public BossAttack[] m_Attacks;

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
            attack.m_BossHittable = m_BossHittable;
        }
	}

    public void LaunchCombo()
    {
        m_Callback.OnComboStart(this);

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

    public void OnAttackEndUnsuccessfully(BossAttack attack)
    {
        // @todo might do something more sophisticated such as switch to a different attack?
        // (tbd when there is more than one kind of attack ;) )
        OnAttackEnd(attack);
    }

    public bool OnHit(Damage dmg)
    { 
        return false;
    }

    private void ParryAttack()
    {
        // @todo separate animation!
        m_Animator.SetTrigger("MeleeDownswingTrigger");

        if (m_AttackTimer != null)
            StopCoroutine(m_AttackTimer);

        if (m_CurrentAttack != null)
        {
            m_CurrentAttack.CancelAttack();
        }
        m_BossHittable.RegisterInterject(this);

        m_Callback.OnActivateParry(this);
    }

    public void OnParryPlayerAttack(BossAttack attack)
    {
        ParryAttack();
    }

    public void OnBlockPlayerAttack(BossAttack attack)
    {
        // @todo some animation? could be difficult for all attacks, maybe let attacks handle this...
    }

    public void OnAttackParried(BossAttack attack)
    {
        if (!m_BetweenAttacks && m_CurrentAttack != null)
        {
            m_CurrentAttack.CancelAttack();
        }
        m_BossStagger.DoStagger();
        StartCoroutine(WaitAfterParried());
    }

    public void CancelCombo()
    {
        if (!m_BetweenAttacks && m_CurrentAttack != null)
        {
            m_CurrentAttack.CancelAttack();
        }

        StopAllCoroutines();
    }

    private IEnumerator WaitAfterParried()
    {
        yield return new WaitForSeconds(1f);

        m_Callback.OnInterruptCombo(this);
    }

    public void OnAttackInterrupted(BossAttack attack)
    {
        OnAttackParried(attack);
    }

    public interface ComboCallback
    {
        void OnComboStart(AttackCombo combo);
        void OnComboEnd(AttackCombo combo);

        void OnActivateParry(AttackCombo combo);
        void OnInterruptCombo(AttackCombo combo);
    }

}

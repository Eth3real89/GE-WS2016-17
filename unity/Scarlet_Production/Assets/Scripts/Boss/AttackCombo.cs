using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCombo : MonoBehaviour, BossAttack.AttackCallback {

    public GameObject m_Boss;
    public Animator m_Animator;
    public TurnTowardsScarlet m_FullTurnCommand;
    public WerewolfHittable m_BossHittable;
    public BossStaggerCommand m_BossStagger;

    public BossMoveCommand m_MoveCommand;

    public BossAttack[] m_Attacks;

    public float m_TimeAfterCombo;

    public BlockingBehaviour m_BlockingBehaviour;
    public int m_MaxBlocksBeforeParry = 3;

    public ComboCallback m_Callback;
    private BossAttack m_CurrentAttack;
    private int m_CurrentAttackIndex;

    private bool m_BetweenAttacks;
    private IEnumerator m_AttackTimer;

    private IEnumerator m_ParriedTimer;

   
	void Start ()
    {
		foreach(BossAttack attack in m_Attacks)
        {
            attack.m_Boss = m_Boss;
            attack.m_Animator = m_Animator;
            attack.m_Callback = this;
            attack.m_BossHittable = m_BossHittable;
            attack.m_FullTurnCommand = m_FullTurnCommand;
        }
	}

    public void LaunchCombo()
    {
        MLog.Log(LogType.BattleLog, 1, "Launching Combo, Combo, " + this);

        if (m_BlockingBehaviour != null)
            m_BlockingBehaviour.m_TimesBlockBeforeParry = m_MaxBlocksBeforeParry;

        m_Callback.OnComboStart(this);

        m_CurrentAttackIndex = 0;
        m_Attacks[m_CurrentAttackIndex].StartAttack();
    }

    public void OnAttackStart(BossAttack attack)
    {
        MLog.Log(LogType.BattleLog, 1, "Attack Start, Combo, " + this);

        if (m_CurrentAttack != null)
            attack.CancelAttack();
        else
        {
            m_BetweenAttacks = false;
            m_CurrentAttack = attack;
        }
    }

    public void OnAttackEnd(BossAttack attack)
    {
        MLog.Log(LogType.BattleLog, 1, "Attack End, Combo, " + this);

        m_BetweenAttacks = true;
        m_CurrentAttack = null;

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

        if (m_CurrentAttackIndex < m_Attacks.Length)
            m_Attacks[m_CurrentAttackIndex].StartAttack();
    }

    public void OnAttackEndUnsuccessfully(BossAttack attack)
    {
        OnAttackEnd(attack);
    }

    public void OnBlockPlayerAttack(BossAttack attack)
    {
        MLog.Log(LogType.BattleLog, 1, "Blocked Player Attack, Combo, " + this);

        m_Animator.SetTrigger("BlockTrigger");
        if (m_MoveCommand != null)
            m_MoveCommand.StopMoving();

        if (m_AttackTimer != null)
            StopCoroutine(m_AttackTimer);

        if (m_CurrentAttack != null)
        {
            m_CurrentAttack.CancelAttack();
        }

        m_Callback.OnActivateBlock(this);
    }

    public void OnAttackParried(BossAttack attack)
    {
        MLog.Log(LogType.BattleLog, 1, "Attack was Parried, Combo, " + this);

        m_Callback.OnComboParried(this);

        if (m_CurrentAttack != null)
        {
            m_CurrentAttack.CancelAttack();
        }
        m_BossStagger.DoStagger();
        m_ParriedTimer = WaitAfterParried();
        StartCoroutine(m_ParriedTimer);
    }

    public void OnAttackRiposted(BossAttack attack)
    {
        MLog.Log(LogType.BattleLog, 1, "Attack was Riposted, Combo, " + this);

        if (m_ParriedTimer != null)
            StopCoroutine(m_ParriedTimer);

        m_Callback.OnComboRiposted(this);

        m_BossStagger.DoStagger("RipostedTrigger");
        m_BossHittable.RegisterInterject(null);

        // @todo make method WaitAfterRiposted
        m_ParriedTimer = WaitAfterParried();
        StartCoroutine(m_ParriedTimer);
    }

    public void CancelCombo()
    {
        MLog.Log(LogType.BattleLog, 1, "Cancelling Combo, Combo, " + this);

        if (m_CurrentAttack != null)
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
        MLog.Log(LogType.BattleLog, 1, "Attack was Interrupted, Combo, " + this);

        m_Callback.OnInterruptCombo(this);
    }

    public interface ComboCallback
    {
        void OnComboStart(AttackCombo combo);
        void OnComboEnd(AttackCombo combo);

        /// <summary>
        ///  Caution! OnInterruptCombo will most likely be called afterwards, the combo should take care of staggering!
        ///  Only if an immediate notification is required!!
        /// </summary>
        /// <param name="combo"></param>
        void OnComboParried(AttackCombo combo);
        void OnComboRiposted(AttackCombo combo);

        void OnActivateBlock(AttackCombo combo);
        void OnInterruptCombo(AttackCombo combo);
    }

}

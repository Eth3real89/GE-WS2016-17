using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelCombo : AttackCombo, BossAttack.AttackCallback {

    /// <summary>
    /// Needs to be at least as long as m_Attacks - 1!!
    /// </summary>
    public float[] m_WaitTimes;

    private HashSet<BossAttack> m_OpenAttacks;

    public override void LaunchCombo()
    {
        m_OpenAttacks = new HashSet<BossAttack>();
        base.LaunchCombo();
    }

    public new void OnAttackStart(BossAttack attack)
    {
        m_BetweenAttacks = false;

        m_CurrentAttackIndex++;

        if (m_Attacks.Length > m_CurrentAttackIndex)
        {
            if (m_WaitTimes[m_CurrentAttackIndex - 1] <= 0)
            {
                m_Attacks[m_CurrentAttackIndex].StartAttack();
            }
            else
            {
                m_AttackTimer = StartNextAttackAfter(m_WaitTimes[m_CurrentAttackIndex - 1]);
                StartCoroutine(m_AttackTimer);
            }
        }

        m_OpenAttacks.Add(attack);
    }

    public new void OnAttackEnd(BossAttack attack)
    {
        if (attack == m_Attacks[m_Attacks.Length - 1])
        {
            base.OnAttackEnd(attack);
        }

        m_OpenAttacks.Remove(attack);
    }

    public new void OnAttackEndUnsuccessfully(BossAttack attack)
    {
        base.OnAttackEndUnsuccessfully(attack);
        if (m_AttackTimer != null)
            StopCoroutine(m_AttackTimer);
    }

    public new void OnAttackInterrupted(BossAttack attack)
    {
        base.OnAttackInterrupted(attack);
        if (m_AttackTimer != null)
            StopCoroutine(m_AttackTimer);
    }

    public new void OnAttackParried(BossAttack attack)
    {
        base.OnAttackParried(attack);
        if (m_AttackTimer != null)
            StopCoroutine(m_AttackTimer);
    }

    public override void CancelCombo()
    {
        foreach(BossAttack attack in m_Attacks)
        {
            attack.CancelAttack();
        }

        if (m_AttackTimer != null)
            StopCoroutine(m_AttackTimer);
    }

}

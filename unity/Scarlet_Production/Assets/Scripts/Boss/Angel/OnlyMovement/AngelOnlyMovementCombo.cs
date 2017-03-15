using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AngelOnlyMovementCombo : AngelCombo {

    public enum MovementComboType { ReachScarlet, Feint, MoveAway };
    public MovementComboType m_MovementComboType;
    
    public override void OnAttackEnd(BossAttack attack)
    {
        m_BetweenAttacks = true;
        _m_CurrentAttack = null;

        m_CurrentAttackIndex++;
        if (m_CurrentAttackIndex >= m_Attacks.Length)
        {
            DetermineSuccessAndEnd(attack);
        }
        else if (!m_Cancelled)
        {
            m_AttackTimer = StartNextAttackAfter(m_Attacks[m_CurrentAttackIndex - 1].m_TimeAfterAttack);
            StartCoroutine(m_AttackTimer);
        }
    }

    protected void DetermineSuccessAndEnd(BossAttack attack)
    {
        if (ManeuverSuccesful())
            m_Success = 1;
        else
            m_Success = -1;

        m_Callback.OnComboEnd(this);
    }

    protected abstract bool ManeuverSuccesful();

    public abstract bool AlreadyInGoodPosition();

    protected virtual bool CloseToScarlet()
    {
        return false;
    }

    protected virtual bool FeintSuccessful()
    {
        return false;
    }

    protected virtual bool FarAwayFromScarlet()
    {
        return false;
    }
}

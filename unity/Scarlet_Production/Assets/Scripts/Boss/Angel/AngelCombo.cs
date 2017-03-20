using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelCombo : AttackCombo, AngelAttack.AngelAttackCallback {

    public enum ComboType {MainRegular, MainFinisher, MainSuper, SideAttack };

    public AngelWeapons.Tips m_AssociatedTip;

    public ComboType m_ComboType;

    public int m_ActualAttackStartIndex = 1;

    public int m_Success;
    public bool m_SkipFind;

    public override void LaunchCombo()
    {
        m_Cancelled = false;
        _m_CurrentAttack = null;

        m_Success = -1;
        MLog.Log(LogType.BattleLog, 1, "Launching Combo, Combo, " + this);

        m_Callback.OnComboStart(this);

        if (!m_Cancelled)
        {
            m_CurrentAttackIndex = m_SkipFind ? 1 : 0;
            m_Attacks[m_CurrentAttackIndex].StartAttack();
        }
    }

    public virtual void LaunchComboFrom(int index)
    {
        m_Cancelled = false;
        _m_CurrentAttack = null;

        m_Success = -1;
        MLog.Log(LogType.BattleLog, 1, "Launching Combo at index: " + index + ", Combo, " + this);

        m_Callback.OnComboStart(this);

        m_CurrentAttackIndex = index;

        if (!m_Cancelled)
        {
            m_Attacks[m_CurrentAttackIndex].StartAttack();
        }
    }

    protected override void SetupAttack(BossAttack attack)
    {
        base.SetupAttack(attack);
        if (attack is AngelAttack)
            ((AngelAttack)attack).m_SuccessCallback = this;
    }

    public virtual void ReportResult(AngelAttack attack)
    {
        m_Success = attack.m_SuccessLevel;
    }

    protected virtual void OnFindFinish(BossAttack findAttack)
    {
        if (m_Success < 0)
        {
            m_Callback.OnComboEnd(this);
        }
        else
        {
            base.OnAttackEnd(findAttack);
        }
    }

}

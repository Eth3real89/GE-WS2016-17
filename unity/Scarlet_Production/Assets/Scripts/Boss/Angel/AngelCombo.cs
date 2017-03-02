using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelCombo : AttackCombo, AngelAttack.AngelAttackCallback {

    public enum ComboType {MainRegular, MainFinisher, MainSuper, SideAttack };

    public AngelWeapons.Tips m_AssociatedTip;

    public ComboType m_ComboType;

    public int m_Success;
    public bool m_SkipFind;

    public override void LaunchCombo()
    {
        m_Success = -1;
        MLog.Log(LogType.BattleLog, 1, "Launching Combo, Combo, " + this);

        m_Callback.OnComboStart(this);

        if (!m_Cancelled)
        {
            m_CurrentAttackIndex = m_SkipFind ? 1 : 0;
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

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelCombo : AttackCombo, AngelAttack.AngelAttackCallback {

    public enum ComboType {MainRegular, MainFinisher, MainSuper, SideAttack };

    public AngelWeapons.Tips m_AssociatedTip;

    public ComboType m_ComboType;

    public int m_Success;

    public override void LaunchCombo()
    {
        m_Success = -1;
        base.LaunchCombo();
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

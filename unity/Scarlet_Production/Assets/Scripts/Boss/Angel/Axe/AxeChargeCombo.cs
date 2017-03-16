using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeChargeCombo : AngelCombo
{

    public override void ReportResult(AngelAttack attack)
    {
        base.ReportResult(attack);
    }

    public override void OnAttackEnd(BossAttack attack)
    {
        if (attack is AxeChargeAttack)
        {
            if (((AngelAttack)attack).m_SuccessLevel >= 1)
            {
                base.OnAttackEnd(attack);
            }
            else
            {
                AngelSoundPlayer.PlayHeavyAttackSound();
                _m_CurrentAttack = null;
                m_CurrentAttackIndex = 2;
                m_Attacks[m_CurrentAttackIndex].StartAttack();
            }
        }
        else if (attack is AxeChargeFollowUpSuccess)
        {
            m_Callback.OnComboEnd(this);
        }
        else
        {
            base.OnAttackEnd(attack);
        }
    }


}

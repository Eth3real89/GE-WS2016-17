using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheLeapCombo : AngelCombo {
    
    public override void ReportResult(AngelAttack attack)
    {
        base.ReportResult(attack);
        
        if (m_CurrentAttackIndex == 1)
        {
            if (m_Success < 1)
            {
                attack.CancelAttack();
                m_Animator.SetTrigger("IdleTrigger");

                base.OnAttackEnd(m_Attacks[m_CurrentAttackIndex]);
            }
        }
    }

    public override void OnAttackEnd(BossAttack attack)
    {
        if (attack is ChaseAttack)
        {
            if (m_Success < 0)
            {
                m_Callback.OnComboEnd(this);
            }
            else
            {
                base.OnAttackEnd(attack);
            }
        }
        else if (attack is ScytheLeapSuperAttack)
        {
            if (m_Success >= 1)
            {
                m_Callback.OnComboEnd(this);
            }
        }
        else
        {
            base.OnAttackEnd(attack);
        }
    }

}

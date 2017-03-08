using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheReapCombo : AngelCombo {
    
    public override void ReportResult(AngelAttack attack)
    {
        base.ReportResult(attack);
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
        else if (attack is ScytheReapAttack)
        {
            if (m_Success < 1)
            {
                m_Callback.OnComboEnd(this);
            }
            else
            {
                base.OnAttackEnd(attack);
            }
        }
        else
        {
            base.OnAttackEnd(attack);
        }
    }

}

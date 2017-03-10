using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeSkullCrusherCombo : AngelCombo {

    public override void ReportResult(AngelAttack attack)
    {
        if (attack is AxeSkullCrusherAttack)
        {
            if (((AxeSkullCrusherAttack)attack).m_SuccessLevel < 1)
            {
                base.OnAttackEnd(attack);
            }
        }

        base.ReportResult(attack);
    }

    public override void OnAttackEnd(BossAttack attack)
    {
        if (attack is ChaseAttack)
        {
            OnFindFinish(attack);
        }
        else if (attack is AxeSkullCrusherAttack)
        {
            if (m_Success != 1)
            {
                // base.OnAttackEnd(attack);
                // do nothing, Blast Wave Attack will take care of this
            }
            else
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

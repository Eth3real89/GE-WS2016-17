using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearTripleStrikeCombo : AngelCombo {

    public override void OnAttackEnd(BossAttack attack)
    {
        if (attack is ChaseAttack)
        {
            base.OnFindFinish(attack);
        }
        else if (attack is HardSpearAttack)
        {
            if (((AngelAttack)attack).m_SuccessLevel >= 1)
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

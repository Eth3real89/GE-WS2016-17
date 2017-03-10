using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeTwoHitsCombo : AngelCombo {

    public override void OnAttackEnd(BossAttack attack)
    {
        if (m_CurrentAttackIndex == 2)
        {
            if (((AngelAttack) m_Attacks[m_CurrentAttackIndex]).m_SuccessLevel == 1)
            {
                m_Animator.SetTrigger("AxeBackhandToFinisherStanceTrigger");
                base.OnAttackEnd(attack);
            }
            else
            {
                m_Animator.SetTrigger("AxeBackhandToIdleTrigger");
                m_Success = 0;
                base.OnAttackEnd(attack);
            }
        }
        else
        {
            base.OnAttackEnd(attack);
        }
    }

}

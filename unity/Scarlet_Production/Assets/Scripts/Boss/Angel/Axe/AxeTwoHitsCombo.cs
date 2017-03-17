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
                float actualEndTime = m_TimeAfterCombo;
                m_TimeAfterCombo = 0.3f;
                base.OnAttackEnd(attack);
                m_TimeAfterCombo = actualEndTime;
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

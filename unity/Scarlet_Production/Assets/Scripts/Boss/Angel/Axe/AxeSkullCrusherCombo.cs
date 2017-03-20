using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeSkullCrusherCombo : AngelCombo {

    public override void LaunchComboFrom(int index)
    {
        if (index == 2)
        {
            _m_CurrentAttack = null;
            m_Cancelled = false;
            m_Callback.OnComboStart(this);
            if (m_Cancelled)
                return;

            m_CurrentAttackIndex = 1;
            string before = ((AngelOnlyAnimationAttack)m_Attacks[1]).m_AnimName;
            ((AngelOnlyAnimationAttack)m_Attacks[1]).m_AnimName = "AxeBackhandToFinisherStanceTrigger";
            ((AngelOnlyAnimationAttack)m_Attacks[1]).StartAttack();
            ((AngelOnlyAnimationAttack)m_Attacks[1]).m_AnimName = before;
        }
        else
        {
            base.LaunchComboFrom(index);
        }
    }

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
        if (m_Cancelled)
            return;

        if (attack is ChaseAttack)
        {
            OnFindFinish(attack);
        }
        else if (attack is AxeSkullCrusherAttack)
        {
            if (m_Success < 1)
            {
                CameraController.Instance.Shake();
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

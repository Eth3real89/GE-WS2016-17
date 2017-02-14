using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorFairyController : FairyController {


    protected override bool HandleHitOutsideOfCombo(Damage dmg)
    {
        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        if (IsBackAttack(dmg) && !m_OnlyJustStaggered)
        {
            MLog.Log(LogType.BattleLog, 0, "Back Attack! " + this);

            dmg.OnSuccessfulHit();

            if (m_TimeWindowManager != null)
            {
                m_TimeWindowManager.Activate(this);
                m_BossHittable.RegisterInterject(m_TimeWindowManager);
            }
            return false;
        }
        else
        {
            dmg.OnBlockDamage();

            m_BlockingBehaviour.Activate(this);
            m_BossHittable.RegisterInterject(m_BlockingBehaviour);

            return true;
        }
    }

}

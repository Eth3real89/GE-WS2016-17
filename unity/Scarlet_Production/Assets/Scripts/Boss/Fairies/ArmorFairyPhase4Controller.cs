using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorFairyPhase4Controller : ArmorFairyController {

    public CharacterHealth m_ArmorHealth;

    protected override bool HandleHitDuringCombo(Damage dmg)
    {
        if (dmg.m_Type == Damage.DamageType.Riposte)
        {
            CancelComboIfActive();

            dmg.OnSuccessfulHit();
            m_TimeWindowManager.ActivateViaRiposte(this);
            m_BossHittable.RegisterInterject(m_TimeWindowManager);

            return false;
        }
        else
        {
            return HandleArmorPhase4Hit(dmg);
        }
    }

    protected virtual bool HandleArmorPhase4Hit(Damage dmg)
    {
        if (IsBackAttack(dmg) && !m_OnlyJustStaggered)
        {
            MLog.Log(LogType.BattleLog, 0, "Back Attack! " + this);
            CancelComboIfActive();

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
            dmg.OnSuccessfulHit();
            m_ArmorHealth.m_CurrentHealth -= dmg.DamageAmount() / 2f;

            return true;
        }
    }

    protected override bool HandleHitOutsideOfCombo(Damage dmg)
    {
        return HandleArmorPhase4Hit(dmg);
    }

}

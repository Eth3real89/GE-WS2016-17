using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorFairyController : FairyController {

    public ArmorFairyParryDamage m_ParryDamage;

    public override void Initialize(FairyControllerCallbacks callbacks)
    {
        base.Initialize(callbacks);
    }

    protected override bool HandleHitOutsideOfCombo(Damage dmg)
    {
        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        CancelComboIfActive();

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

    public override void OnBossParries()
    {
        CancelComboIfActive();

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        MLog.Log(LogType.FairyLog, 1, "Boss Parries: Armor " + this);

        Hittable hittable = m_Scarlet.GetComponent<Hittable>();
        if (hittable != null)
        {
            hittable.Hit(m_ParryDamage);
        }

        m_NextComboTimer = StartNextComboAfter(1f);
        StartCoroutine(m_NextComboTimer);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPlaceAEAttack : AEAttack, FixedPlaceSetupVisuals.SetupCallback {

    public Damage m_AEDamage;

    public FixedPlaceSetupVisuals m_SetupVisuals;
    public FixedPlaceAttackVisuals m_AttackVisuals;

    public override void StartAttack()
    {
        base.StartAttack();

        m_SetupVisuals.Show(this);
    }

    public void OnSetupOver()
    {
        m_Callback.OnAttackEnd(this);
    }
}

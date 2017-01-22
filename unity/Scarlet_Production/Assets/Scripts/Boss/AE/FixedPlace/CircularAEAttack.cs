using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularAEAttack : FixedPlaceAEAttack {

    public float m_SetupTime = 2;
    public float m_Size = 5;
    public float m_Angle = 270;

    public float m_DamageAmount;

    public override void StartAttack()
    {
        CircularSetupVisuals visuals = (CircularSetupVisuals)m_SetupVisuals;
        visuals.m_Angle = m_Angle;
        visuals.m_Size = m_Size;
        visuals.m_TimeShown = m_SetupTime;

        CircularAttackVisuals attackVisuals = (CircularAttackVisuals)m_AttackVisuals;
        attackVisuals.m_Size = m_Size;
        attackVisuals.m_Angle = m_Angle;

        m_AEDamage.m_DamageAmount = m_DamageAmount;

        base.StartAttack();
    }

}

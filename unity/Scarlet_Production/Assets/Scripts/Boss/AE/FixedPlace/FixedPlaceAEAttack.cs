using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPlaceAEAttack : AEAttack, FixedPlaceSetupVisuals.SetupCallback
{

    public AEAttackDamage m_AEDamage;

    public float m_AttackTime = 0.5f;

    private bool m_Active = false;
    private bool m_DamageActive = false;

    public FixedPlaceSetupVisuals m_SetupVisuals;
    public FixedPlaceAttackVisuals m_AttackVisuals;

    public TurnTowardsScarlet m_TurnTowardsScarlet;

    public override void StartAttack()
    {
        base.StartAttack();

        m_SetupVisuals.Show(this);
    }

    public void OnSetupOver()
    {
        m_SetupVisuals.Hide();
        m_AttackVisuals.Show();

        StartCoroutine(DisableAttackTimer());
        m_AEDamage.Activate();
    }

    private IEnumerator DisableAttackTimer()
    {
        yield return new WaitForSeconds(m_AttackTime);

        m_Active = false;
        m_AttackVisuals.Hide();
        m_AEDamage.m_Active = false;

        m_Callback.OnAttackEnd(this);
    }

}

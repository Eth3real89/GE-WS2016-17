using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The boss is in a fixed place and may or may not rotate an attack towards scarlet.
/// </summary>
public class FixedPlaceAEAttack : AEAttack, FixedPlaceSetupVisuals.SetupCallback
{

    public AEAttackDamage m_AEDamage;

    public float m_AttackTime = 0.5f;

    public FixedPlaceSetupVisuals m_SetupVisuals;
    public FixedPlaceAttackVisuals m_AttackVisuals;

    public TurnTowardsScarlet m_TurnTowardsScarlet;
    public float m_TrackingSpeedAngles;
    private float m_PreviousSpeed;

    private IEnumerator m_AttackTimer;

    private IEnumerator m_SetupTimer;
    private bool m_SetupActive = false;

    public override void StartAttack()
    {
        base.StartAttack();

        m_SetupActive = true;

        m_PreviousSpeed = m_TurnTowardsScarlet.m_TurnSpeed;

        m_TurnTowardsScarlet.m_TurnSpeed = 9999;
        m_TurnTowardsScarlet.DoTurn();

        m_TurnTowardsScarlet.m_TurnSpeed = m_TrackingSpeedAngles;

        m_SetupVisuals.ShowSetup(this);

        m_SetupTimer = TurnDuringSetup();
        StartCoroutine(m_SetupTimer);
    }

    private IEnumerator TurnDuringSetup()
    {
        while (m_SetupActive)
        {
            m_TurnTowardsScarlet.DoTurn();
            yield return null;
        }
        m_TurnTowardsScarlet.m_TurnSpeed = m_PreviousSpeed;
    }

    public void OnSetupOver()
    {
        m_SetupActive = false;
        m_SetupVisuals.HideAttack();
        m_AttackVisuals.ShowAttack();

        m_AttackTimer = DisableAttackTimer();
        StartCoroutine(m_AttackTimer);
        m_AEDamage.Activate();
    }

    public override void CancelAttack()
    {
        base.CancelAttack();

        m_SetupActive = false;

        m_SetupVisuals.HideAttack();

        if (m_AttackTimer != null)
            StopCoroutine(m_AttackTimer);

        if (m_SetupTimer != null)
            StopCoroutine(m_AttackTimer);

        m_AEDamage.m_Active = false;
        HideLightGuard();
        m_AttackVisuals.HideAttack();
    }

    private IEnumerator DisableAttackTimer()
    {
        yield return new WaitForSeconds(m_AttackTime);

        m_AttackVisuals.HideAttack();
        m_AEDamage.m_Active = false;

        m_Callback.OnAttackEnd(this);
        HideLightGuard();
    }

}

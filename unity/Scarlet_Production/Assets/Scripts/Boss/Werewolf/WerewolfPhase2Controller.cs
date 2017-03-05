using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfPhase2Controller : WerewolfController
{

    public CharacterHealth m_BossHealth;

    private bool m_Active = false;

    private BossfightCallbacks m_Callbacks;
    
    private void Update()
    {
        if (m_Active)
        {
            if (m_BossHealth != null && m_BossHealth.m_CurrentHealth <= 0)
            {
                EndPhase();
            }
        }
    }

    public void LaunchPhase(BossfightCallbacks callbacks)
    {
        m_NotDeactivated = true;

        m_Callbacks = callbacks;
        m_Active = true;

        m_BossHittable.RegisterInterject(this);

        RegisterComboCallback();
        RegisterEventsForSound();

        m_CurrentComboIndex = 0;
        StartCoroutine(StartAfterDelay());
    }

    protected override void OnJumpStart()
    {
        base.OnJumpStart();
        PlayLightAttackSound();
    }

    protected override void OnMeleeDownswing()
    {
        base.OnMeleeDownswing();
        bool heavyAttack = false;
        // determine if it is a heavy attack based on dmg. Heavy attacks are defined by their damage.

        if (m_ActiveCombo != null) // pretty much has to be the case, but just to be sure
        {
            BossAttack attack = m_ActiveCombo.m_CurrentAttack;
            if (attack is ChaseAttack)
            {
                if (((ChaseAttack)attack).m_DamageAmount > 31f)
                    heavyAttack = true;
            }
            else if (attack is RepeatedMeleeAttack)
            {
                if (((RepeatedMeleeAttack)attack).m_DamageAmount > 31f)
                    heavyAttack = true;
            }

            if (heavyAttack)
            {
                PlayHeavyAttackSound();
            }
            else
            {
                PlayLightAttackSound();
            }
        }
    }

    private void EndPhase()
    {
        m_NotDeactivated = false;
        CancelComboIfActive();

        m_Active = false;
        m_BossHealth.m_CurrentHealth = m_BossHealth.m_MaxHealth;

        UnRegisterEventsForSound();

        WerewolfHittable hittable = FindObjectOfType<WerewolfHittable>();
        if (hittable != null)
            hittable.StopPlayingCriticalHPSound();

        m_Callbacks.PhaseEnd(this);
    }

}

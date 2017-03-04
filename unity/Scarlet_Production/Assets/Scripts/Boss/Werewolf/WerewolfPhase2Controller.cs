using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfPhase2Controller : BossController {

    public CharacterHealth m_BossHealth;

    private bool m_Active = false;

    private BossfightCallbacks m_Callbacks;
    
    private void Update()
    {
        if (m_Active)
        {
            if (m_BossHealth != null && m_BossHealth.m_CurrentHealth <= 0)
            {
                m_NotDeactivated = false;
                CancelComboIfActive();

                m_Active = false;
                m_BossHealth.m_CurrentHealth = m_BossHealth.m_MaxHealth;

                m_Callbacks.PhaseEnd(this);
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

        m_CurrentComboIndex = 0;
        StartCoroutine(StartAfterDelay());
    }

}

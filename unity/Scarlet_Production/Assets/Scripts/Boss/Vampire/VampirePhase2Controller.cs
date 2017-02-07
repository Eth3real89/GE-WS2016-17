﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampirePhase2Controller : VampireController
{

    private bool m_EndInitialized;
    public CharacterHealth m_BossHealth;

    public override void StartPhase(BossfightCallbacks callbacks)
    {
        m_EndInitialized = false;
        base.StartPhase(callbacks);

        StartCoroutine(StartAfterDelay());
    }

    private void Update()
    {
        if (m_BossHealth.m_CurrentHealth == 0 && !m_EndInitialized)
        {
            if (m_ActiveCombo != null)
                m_ActiveCombo.CancelCombo();

            StopAllCoroutines();

            m_BossHealth.m_CurrentHealth = m_BossHealth.m_HealthStart;
            m_Callback.PhaseEnd(this);
        }
    }
}

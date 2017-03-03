using System.Collections;
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

        m_LightGuard.ReattachVisualsToParent();

        m_CurrentComboIndex = -1;
        m_NextComboTimer = StartNextComboAfter(0.5f);
        StartCoroutine(m_NextComboTimer);
    }

    private void Update()
    {
        if (m_BossHealth.m_CurrentHealth <= 0 && !m_EndInitialized)
        {
            if (m_ActiveCombo != null)
                m_ActiveCombo.CancelCombo();

            StopAllCoroutines();

            DashTo(m_InLightZones[0], 1f);
            StartCoroutine(EndPhase());
            
            UnRegisterAnimationEvents();
        }
    }

    protected virtual IEnumerator EndPhase()
    {
        m_EndInitialized = true;

        new FARQ().ClipName("vampire").Location(transform).StartTime(101f).EndTime(117.855f).Volume(1).StopIfPlaying();
        float t = 0;
        while ((t += Time.deltaTime) < 1.5f)
        {
            m_BossHealth.m_CurrentHealth += m_BossHealth.m_MaxHealth * (t / 1.5f);
            yield return null;
        }
        m_BossHealth.m_CurrentHealth = m_BossHealth.m_MaxHealth;

        m_Callback.PhaseEnd(this);
    }
}

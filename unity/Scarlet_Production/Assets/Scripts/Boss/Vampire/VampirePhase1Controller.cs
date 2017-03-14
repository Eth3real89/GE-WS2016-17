using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampirePhase1Controller : VampireController
{

    public CharacterHealth m_BossHealth;

    public Transform m_Refuge;

    private bool m_EndInitialized;
    
    public override void StartPhase(BossfightCallbacks callbacks)
    {
        m_EndInitialized = false;
        base.StartPhase(callbacks);

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

            DashTo(m_Refuge, 1f);
            StartCoroutine(EndPhase());
            m_EndInitialized = true;
            UnRegisterAnimationEvents();
        }
    }

    private IEnumerator EndPhase()
    {
        ExtremelyDangerousCancelAllCombosEver();
        m_NotDeactivated = false;

        yield return new WaitForSeconds(1f);
        float t = 0;

        new FARQ().ClipName("vampire").Location(transform).StartTime(101f).EndTime(117.855f).Volume(1).StopIfPlaying();

        while ((t += Time.deltaTime) < 1.5f)
        {
            m_BossHealth.m_CurrentHealth += m_BossHealth.m_MaxHealth * (t / 1.5f);
            yield return null;
        }
        m_BossHealth.m_CurrentHealth = m_BossHealth.m_MaxHealth;
        CancelHitBehaviours();

        m_Callback.PhaseEnd(this);
    }

    protected override void OnBulletAttackStart()
    {
        PlayAttackSound(m_CurrentComboIndex != 3);
        base.OnBulletAttackStart();
    }

    protected override void OnBeamAttackStart()
    {
        PlayAttackSound(false);
        base.OnBeamAttackStart();
    }

    protected override void OnBlastWaveStart()
    {
        PlayAttackSound(true);
        base.OnBlastWaveStart();
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        if (m_EndInitialized)
            return;

        base.OnComboEnd(combo);
    }


}

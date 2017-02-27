using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterPhase3Controller : DemonHunterController {

    public float m_BasicTimeRhythm = 3f;
    public ParallelCombo[] m_CombosForRhythm;

    protected override void StartFirstCombo()
    {
        m_NotDeactivated = true;

       // Time.timeScale = 3f;

        m_NextComboTimer = SetupFinalPhase();
        StartCoroutine(m_NextComboTimer);

        SetRelevantTimesTo(m_BasicTimeRhythm);
    }

    protected virtual IEnumerator SetupFinalPhase()
    {
        m_SkipReload = true;
        m_PreparationRoutine = PreparePistols();
        yield return StartCoroutine(m_PreparationRoutine);

        m_EvasionCommand.EvadeTowards(m_AttackSpots[0], this, this.OnSpotReached());
    }

    protected virtual IEnumerator OnSpotReached()
    {
        yield return null;

        m_CurrentComboIndex = -1;

        m_NextComboTimer = StartNextComboAfter(0.5f);
        StartCoroutine(m_NextComboTimer);
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        yield return new WaitForSeconds(time);

        SetCurrentTargetFromAttack();

        if (m_CurrentComboIndex == 4)
        {
            m_SkipReload = true;
            m_PreparationRoutine = PrepareRifle();
            yield return StartCoroutine(m_PreparationRoutine);
        }
        else
        {
            m_PreparationRoutine = m_EvasionCommand.QuickPerfectRotationRoutine(0.2f, m_PerfectRotationTarget);
            yield return StartCoroutine(m_PreparationRoutine);
        }

        if (m_CurrentComboIndex + 1 >= m_Combos.Length)
        {
            m_PreparationRoutine = PrepareAttack(0);
        }
        else
        {
            m_PreparationRoutine = PrepareAttack(m_CurrentComboIndex + 1);
        }
        yield return StartCoroutine(m_PreparationRoutine);

        StartNextCombo();
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        m_ActiveCombo = null;

        m_NextComboTimer = StartNextComboAfter(combo.m_TimeAfterCombo);
        StartCoroutine(m_NextComboTimer);   
    }

    protected virtual void SetCurrentTargetFromAttack()
    {
        if (m_CurrentComboIndex == -1 || m_CurrentComboIndex == 6 || m_CurrentComboIndex == 7)
        {
            m_PerfectRotationTarget = m_Scarlet.transform;
        }
        else if (m_CurrentComboIndex == 0 || m_CurrentComboIndex == 1 || m_CurrentComboIndex == 4 || m_CurrentComboIndex == 5)
        {
            m_PerfectRotationTarget = ChooseGrenadeSpot(m_Combos[m_CurrentComboIndex + 1]);
        }
        else if (m_CurrentComboIndex == 2 || m_CurrentComboIndex == 3)
        {
            m_PerfectRotationTarget = m_AttackSpots[1];
        }
    }

    protected override Transform ChooseGrenadeSpot(AttackCombo attackCombo)
    {
        for (int i = 0; i < attackCombo.m_Attacks.Length; i++)
        {
            if (attackCombo.m_Attacks[i] is BulletAttack)
            {
                BulletMovement m = (((BulletAttack)attackCombo.m_Attacks[i]).m_BaseSwarm.m_Invoker.m_Factories[0].m_Movement);
                if (m is BulletGrenadeMovement)
                {
                    return ((BulletGrenadeMovement)m).m_Goal;
                }
            }
        }

        return null;
    }

    public override bool OnHit(Damage dmg)
    {
        return true;
    }

    protected void SetRelevantTimesTo(float m_BasicTimeRhythm)
    {
        int[] m_Repetitions = { 40, 36, 32, 20, 18 };

        for (int i = 0; i < m_CombosForRhythm.Length; i++)
        {
            BossAttack[] attacks = new BossAttack[m_Repetitions[i]];

            attacks[0] = m_CombosForRhythm[i].m_Attacks[0];
            for (int j = 0; j < m_Repetitions[i] - 2; j++)
            {
                attacks[j + 1] = m_CombosForRhythm[i].m_Attacks[1 + j % 4];
            }

            attacks[m_Repetitions[i] - 1] = m_CombosForRhythm[i].m_Attacks[m_CombosForRhythm[i].m_Attacks.Length - 1];

            m_CombosForRhythm[i].m_Attacks = attacks;
        }

        foreach (ParallelCombo combo in m_CombosForRhythm)
        {
            combo.m_WaitTimes = new float[combo.m_Attacks.Length - 1];

            for (int i = 0; i < combo.m_WaitTimes.Length; i++)
            {
                combo.m_WaitTimes[i] = m_BasicTimeRhythm;
            }

            combo.m_TimeAfterCombo = m_BasicTimeRhythm;


        }
    }


}

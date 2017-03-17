using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterPhase3Controller : DemonHunterController {

    public float m_BasicTimeRhythm = 3f;
    public ParallelCombo[] m_CombosForRhythm;

    protected bool m_CanDie;

    protected override void StartFirstCombo()
    {
        m_NotDeactivated = true;
        m_CanDie = false;

       // Time.timeScale = 3f;

        m_NextComboTimer = SetupFinalPhase();
        StartCoroutine(m_NextComboTimer);

        SetWaveTimes(m_BasicTimeRhythm);
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

        if (m_CurrentComboIndex == 8)
        {
            m_NextComboTimer = InitEnd();
        }
        else
        {
            m_NextComboTimer = StartNextComboAfter(combo.m_TimeAfterCombo);
        }

        StartCoroutine(m_NextComboTimer);   
    }

    protected virtual IEnumerator InitEnd()
    {
        ParallelCombo[] combos = FindObjectsOfType<ParallelCombo>();
        foreach(ParallelCombo c in combos)
        {
            try
            {
                c.CancelCombo();
            }
            catch { /* well then it probably wasn't active :> */ }
        }

        m_CanDie = true;
        yield return new WaitForSeconds(3f);
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
        if (m_CanDie)
        {
            m_BossHittable.m_Health.m_CurrentHealth = -1f;

            CameraController.Instance.Shake();
            m_NotDeactivated = false;
            StopAllCoroutines();
            m_Callback.PhaseEnd(this);
            
            return true;
        }
        else
        {
            return true;
        }
    }

    public override void OnComboStart(AttackCombo combo)
    {
        if (m_CurrentComboIndex == 1 || m_CurrentComboIndex == 2 || m_CurrentComboIndex == 5 || m_CurrentComboIndex == 6)
        {
            PlayGrenadeSound();
        }

        base.OnComboStart(combo);
    }

    protected void SetWaveTimes(float m_BasicTimeRhythm)
    {
        int[] m_Repetitions = { 41, 37, 33, 21, 19 };

        // Basic Setup:

        for (int i = 0; i < m_CombosForRhythm.Length; i++)
        {
            BossAttack[] attacks = new BossAttack[m_Repetitions[i]];

            // wave hints, explosions & light guards will be re-used:
            attacks[0] = m_CombosForRhythm[i].m_Attacks[0];
            attacks[1] = m_CombosForRhythm[i].m_Attacks[1];
            attacks[2] = m_CombosForRhythm[i].m_Attacks[2];
            for (int j = 2; j < m_Repetitions[i] - 2; j++)
            {
                attacks[j + 1] = m_CombosForRhythm[i].m_Attacks[3 + j % 4];
            }

            attacks[m_Repetitions[i] - 1] = m_CombosForRhythm[i].m_Attacks[m_CombosForRhythm[i].m_Attacks.Length - 1];

            m_CombosForRhythm[i].m_Attacks = attacks;
        }

        foreach (ParallelCombo combo in m_CombosForRhythm)
        {
            combo.m_WaitTimes = new float[combo.m_Attacks.Length - 1];

            combo.m_WaitTimes[0] = 0f;
            for (int i = 2; i < combo.m_WaitTimes.Length; i++)
            {
                combo.m_WaitTimes[i] = m_BasicTimeRhythm;
            }

            combo.m_TimeAfterCombo = m_BasicTimeRhythm;
        }

        // For Fairness: (values via trial & error), slows waves down / makes them not all simultaneous
        m_CombosForRhythm[0].m_WaitTimes[6] = 12f;

        for(int i = 1; i < 8; i++)
        {
            m_CombosForRhythm[1].m_WaitTimes[9 + i] *= 2;
            m_CombosForRhythm[2].m_WaitTimes[8 + i] *= 2;
        }

        for (int i = 0; i < 5; i++)
        {
            int startPoint = (new int[] {16, 18, 17, 2, 2})[i];

            for(int j = startPoint; j < m_CombosForRhythm[i].m_WaitTimes.Length; j++)
            {
                m_CombosForRhythm[i].m_WaitTimes[j] = m_CombosForRhythm[i].m_WaitTimes[j] * (j % 3) + 2.5f;
            }
        }

        // set wave hints:
        for(int i = 0; i < 5; i++)
        {
            int offset = 2;
            RepeatedEffectPlayer hintAttack = (RepeatedEffectPlayer) m_CombosForRhythm[i].m_Attacks[0];
            hintAttack.m_Times = new float[m_CombosForRhythm[i].m_Attacks.Length - offset];
            for(int j = offset; j < m_CombosForRhythm[i].m_WaitTimes.Length; j++)
            {
                hintAttack.m_Times[j - offset] = m_CombosForRhythm[i].m_WaitTimes[j];
            }
            //hintAttack.m_Times[0] = 
        }

    }


}

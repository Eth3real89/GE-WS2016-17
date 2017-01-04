using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfRagemodeController : BossController
{

    private BossfightCallbacks m_Callbacks;

    public AttackCombo m_HitCombo;
    public AttackCombo m_LeapCombo;
    public AttackCombo m_ChaseCombo;

    public TurnTowardsScarlet m_TurnTowardsScarlet;

    public float m_TotalRagemodeTime = 120f;
    private bool m_CancelAfterComboFinishes = false;

    new void Start()
    {
    }

    public void LaunchPhase(BossfightCallbacks callbacks)
    {
        m_Callbacks = callbacks;

        m_Combos = new AttackCombo[3];
        m_Combos[0] = m_HitCombo;
        m_Combos[1] = m_LeapCombo;
        m_Combos[2] = m_ChaseCombo;

        base.RegisterComboCallback();

        StartCoroutine(StartAfterDelay());
        StartCoroutine(RageModeCountdown());
    }

    private void EndRageMode()
    {
        m_Callbacks.PhaseEnd(this);
    }

    private new IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        m_CancelAfterComboFinishes = false;
        DecideNextCombo(null);
    }

    private IEnumerator RageModeCountdown()
    {
        yield return new WaitForSeconds(m_TotalRagemodeTime);
        m_CancelAfterComboFinishes = true;
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        if (m_CancelAfterComboFinishes)
        {
            EndRageMode();
        }
        else
        {
            DecideNextCombo(combo);
        }
    }

    private IEnumerator StartNextComboAfter(float time, AttackCombo combo)
    {
        yield return new WaitForSeconds(time);
        combo.LaunchCombo();
    }

    private void DecideNextCombo(AttackCombo previous)
    {
        AttackCombo newCombo = m_HitCombo;

        float distance = Vector3.Distance(transform.position, m_Scarlet.transform.position);

        if (distance <= 4)
        {
            if (Mathf.Abs(m_TurnTowardsScarlet.CalculateAngleTowardsScarlet()) >= 40)
            {
                newCombo = m_ChaseCombo;
            }
        }
        else
        {
            newCombo = m_LeapCombo;
        }

        StartCoroutine(StartNextComboAfter(0.3f, newCombo));
    }

    public override void OnInterruptCombo(AttackCombo combo)
    {
        OnComboEnd(combo);
    }
}

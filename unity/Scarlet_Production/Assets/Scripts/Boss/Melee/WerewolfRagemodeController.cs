using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfRagemodeController : BossController
{

    public AttackCombo m_HitCombo;
    public AttackCombo m_LeapCombo;
    public AttackCombo m_ChaseCombo;

    public TurnTowardsScarlet m_TurnTowardsScarlet;

    new void Start()
    {
        m_Combos = new AttackCombo[3];
        m_Combos[0] = m_HitCombo;
        m_Combos[1] = m_LeapCombo;
        m_Combos[2] = m_ChaseCombo;

        base.RegisterComboCallback();

        StartCoroutine(StartAfterDelay());
    }

    private IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        DecideNextCombo(null);
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        DecideNextCombo(combo);
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
            else
            {
                print("yaay");
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

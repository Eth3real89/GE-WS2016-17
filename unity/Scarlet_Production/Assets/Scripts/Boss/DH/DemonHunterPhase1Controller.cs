using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterPhase1Controller : DemonHunterController {

    private const float m_FirstAttackShootSpeed = 2f;


    public override void StartPhase(BossfightCallbacks callback)
    {
        base.StartPhase(callback);
    }

    protected override IEnumerator PrepareAttack(int attackIndex)
    {
        yield return base.PrepareAttack(attackIndex);
    }

    public override void OnComboStart(AttackCombo combo)
    {
        m_DHAnimator.SetTrigger("ShootTrigger");
        if (m_CurrentComboIndex == 0)
        {
            m_DHAnimator.SetFloat("ShootingSpeed", m_FirstAttackShootSpeed);
        }


        base.OnComboStart(combo);
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        m_DHAnimator.SetTrigger("StopShootingTrigger");
        if (m_CurrentComboIndex == 0)
        {
            m_DHAnimator.SetFloat("ShootingSpeed", 1f);
        }

        base.OnComboEnd(combo);
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        return base.StartNextComboAfter(time);
    }

}

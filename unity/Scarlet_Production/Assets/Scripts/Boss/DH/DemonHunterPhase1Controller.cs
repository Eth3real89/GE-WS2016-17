using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterPhase1Controller : DemonHunterController {

    private const float m_FirstAttackShootSpeed = 2f;
    private const float m_SecondAttackShootSpeed = 4f;
    private const float m_FifthAttackShootSpeed = 0.3f;

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
        if (m_CurrentComboIndex == 3)
        {
            m_DHAnimator.SetTrigger("PistolsPullTogetherTrigger");
        }
        else if (m_Types[m_CurrentComboIndex] == AttackType.Pistols)
        {
            m_DHAnimator.SetTrigger("ShootTrigger");
        }

        if (m_CurrentComboIndex == 0)
        {
            m_DHAnimator.SetFloat("ShootingSpeed", m_FirstAttackShootSpeed);
        }
        else if (m_CurrentComboIndex == 1)
        {
            m_DHAnimator.SetFloat("ShootingSpeed", m_SecondAttackShootSpeed);
        }
        else if (m_CurrentComboIndex == 5)
        {
            m_DHAnimator.SetFloat("ShootingSpeed", m_FifthAttackShootSpeed);
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

    protected override IEnumerator AfterCombo(AttackCombo combo)
    {
        yield return base.AfterCombo(combo);
    }

}

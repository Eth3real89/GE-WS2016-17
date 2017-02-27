using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterPhase1Controller : DemonHunterController {

    private const float m_FirstAttackShootSpeed = 2f;
    private const float m_SecondAttackShootSpeed = 4f;
    private const float m_FifthAttackShootSpeed = 0.3f;

    protected bool m_EndInitialized;

    public override void StartPhase(BossfightCallbacks callback)
    {
        m_EndInitialized = false;
        base.StartPhase(callback);
    }

    private void Update()
    {
        if (!m_EndInitialized && m_DHHealth.m_CurrentHealth <= 0)
        {
            m_EndInitialized = true;
            m_NotDeactivated = false;
            CancelComboIfActive();
            StopAllCoroutines();
            m_Callback.PhaseEnd(this);
        }
    }

    protected override IEnumerator PrepareAttack(int attackIndex)
    {
        if (attackIndex == 5 || attackIndex == 1)
        {
            GameObject t = GameObject.Find("_MainObject");
            if (t != null)
            {
                m_PerfectRotationTarget = t.transform;
            }
        }
        else
        {
            m_PerfectRotationTarget = m_Scarlet.transform;
        }

        yield return base.PrepareAttack(attackIndex);
    }

    protected override bool InitRangeCheckCondition(int nextCombo)
    {
        return base.InitRangeCheckCondition(nextCombo);
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

}

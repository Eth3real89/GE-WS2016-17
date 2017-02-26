using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterPhase2Controller : DemonHunterController
{


    private const float m_FirstAttackShootSpeed = 2f;
    private const float m_SecondAttackShootSpeed = 4f;
    private const float m_FifthAttackShootSpeed = 0.3f;

    protected bool m_EndInitialized;

    protected List<int> m_LastAttacks;

    public override void StartPhase(BossfightCallbacks callback)
    {
        m_LastAttacks = new List<int>();
        m_EndInitialized = false;

//        Time.timeScale = 3;

        base.StartPhase(callback);
    }

    private void Update()
    {
        if (!m_EndInitialized && m_DHHealth.m_CurrentHealth <= 0.11 * m_DHHealth.m_CurrentHealth) // 0.11: Avoid "rounding" bugs
        {
            m_EndInitialized = true;
            m_NotDeactivated = false;
            CancelComboIfActive();
            StopAllCoroutines();
            m_Callback.PhaseEnd(this);
        }
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        m_TimesFled = 0;

        yield return base.StartNextComboAfter(time);

        if (m_CurrentComboIndex != 4)
        {
            m_LastAttacks.Add(m_CurrentComboIndex > 4? 1 : 0);
        }

        if (m_LastAttacks.Count > 4)
        {
            m_LastAttacks.RemoveAt(0);
        }
    }

    protected override void InitNextAttack()
    {
        DecideOnNextAttack();

        if (m_CurrentComboIndex >= 3)
        {
            MLog.Log(LogType.DHLog, "Starting next combo from AfterCombo, DH, After Grenade, " + this);
            m_NextComboTimer = StartNextComboAfter(0.01f);
            StartCoroutine(m_NextComboTimer);
        }
        else
        {
            MLog.Log(LogType.DHLog, "Starting next combo from AfterCombo, DH, After Evasion, " + this);

            m_DropGrenadeAttack.CancelAttack();
            m_Evading = true;
            m_EvasionCommand.EvadeTowards(GetFurthestSpotFromScarlet(), this, base.OnEvasionFinished());
        }
    }

    protected void DecideOnNextAttack()
    {
        // these names are actually wrong, but that's what it comes down to ~
        int[] runAroundAttacks = { 0, 1, 2, 3 };
        int[] attacksInBubble = { 5, 6, 9};

        if (m_Types[m_CurrentComboIndex] == AttackType.DropGrenade)
        {
            m_CurrentComboIndex = ChooseNextAttackFrom(5, 6, 7) - 1;
                return;
        }
        else if (m_CurrentComboIndex == 7  || m_CurrentComboIndex == 8) // 
        {
            // do nothing, continue to next attack!! (which is what happens if nothing is done here!)
        }
        else
        {
            if (m_CurrentComboIndex >= 5)
            { // attacks in bubble
                if (LastTwoAttacksWereTheSame())
                {
                    m_CurrentComboIndex = ChooseNextAttackFrom(runAroundAttacks) - 1;
                    return;
                }
                else
                {
                    m_CurrentComboIndex = ChooseNextAttackFrom(0, 1, 2, 3, 5, 6, 7) - 1;
                }
            }
            else // pistol / move-around-y atack
            {
                if (LastTwoAttacksWereTheSame())
                {
                    m_CurrentComboIndex = 3;
                }
                else
                {
                    m_CurrentComboIndex = ChooseNextAttackFrom(0, 1, 2, 3, 4) - 1;
                }
            }
        }
    }

    protected bool LastTwoAttacksWereTheSame()
    {
        if (m_LastAttacks.Count < 2)
            return false;

        return m_LastAttacks[m_LastAttacks.Count - 1] == m_LastAttacks[m_LastAttacks.Count - 2];
    }

    protected int ChooseNextAttackFrom(params int[] possibilites)
    {
        return possibilites[UnityEngine.Random.Range(0, possibilites.Length)];
    }

    protected override IEnumerator PrepareAttack(int attackIndex)
    {
        if (attackIndex == 3 || attackIndex == 1)
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

    protected override IEnumerator OnEvasionFinished()
    {
        m_Evading = false;
        yield return null;
        MLog.Log(LogType.DHLog, "On Evasion Finished,  DH, " + this);

        DecideOnNextAttack();

        m_NextComboTimer = StartNextComboAfter(0.01f);
        StartCoroutine(m_NextComboTimer);
    }

    protected override IEnumerator AfterCombo(AttackCombo combo)
    {

        m_TimesFled = 0;

        // if the next attack is "drop grenade": don't even try to flee
        if (m_Combos.Length > m_CurrentComboIndex + 1 && m_Types[m_CurrentComboIndex + 1] != AttackType.DropGrenade)
        {
            m_RangeCheck = RangeCheck();
            StartCoroutine(m_RangeCheck);
        }

        yield return new WaitForSeconds(combo.m_TimeAfterCombo);

        if (combo != m_Combos[m_CurrentComboIndex])
            yield break;

        if (m_RangeCheck != null)
            StopCoroutine(m_RangeCheck);

        if (m_PreparationRoutine != null)
            StopCoroutine(m_PreparationRoutine);

        InitNextAttack();
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

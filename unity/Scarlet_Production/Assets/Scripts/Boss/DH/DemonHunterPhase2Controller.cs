using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterPhase2Controller : DemonHunterController
{

    private const float m_FirstAttackShootSpeed = 2f;
    private const float m_SecondAttackShootSpeed = 4f;
    private const float m_FifthAttackShootSpeed = 0.3f;

    private const int BEFORE_DROP_GRENADE = 3;

    protected bool m_EndInitialized;

    protected List<int> m_LastAttackTypes;
    protected List<int> m_LastActualAttacks;

    public override void StartPhase(BossfightCallbacks callback)
    {
        m_LastAttackTypes = new List<int>();
        m_LastActualAttacks = new List<int>();
        m_EndInitialized = false;

        m_NotDeactivated = true;

//        Time.timeScale = 3;

        base.StartPhase(callback);

        // because this definitely happens after a hit :)
        m_SkipForceKeepWindowOpen = true;
        m_SkipReload = true;
    }

    private void Update()
    {
        if (!m_EndInitialized && m_DHHealth.m_CurrentHealth <= (1 / (float) m_NumHits)  * m_DHHealth.m_MaxHealth + 5) // +5: Avoid "rounding" bugs 
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
        if (m_CurrentComboIndex >= 4 && !m_DropGrenadeAttack.IsActive())
        {
            SetDropGrenadeAttackGoal(m_BossHittable.transform, true);
            MakeProtectiveGrenadeNext();
        }

        yield return base.StartNextComboAfter(time);

        if (m_CurrentComboIndex != 4 && m_CurrentComboIndex != 8 && m_CurrentComboIndex != 9)
            m_LastAttackTypes.Add(m_CurrentComboIndex > 4? 1 : 0);
        
        if (m_LastAttackTypes.Count > 5)
            m_LastAttackTypes.RemoveAt(0);
        
        if (m_CurrentComboIndex != 8 && m_CurrentComboIndex != 9)
            m_LastActualAttacks.Add(m_CurrentComboIndex);

        if (m_LastActualAttacks.Count > 10)
            m_LastActualAttacks.RemoveAt(0);
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

            m_NextComboTimer = EquipPistolsThenRun();
            StartCoroutine(m_NextComboTimer);
        }
    }

    private IEnumerator EquipPistolsThenRun()
    {
        m_SkipReload = true;
        m_PreparationRoutine = PreparePistols();
        yield return StartCoroutine(m_PreparationRoutine);
        m_SkipReload = false;

        m_DropGrenadeAttack.CancelAttack();
        m_Evading = true;
        m_EvasionCommand.EvadeTowards(GetFurthestSpotFromScarlet(), this, base.OnEvasionFinished());
    }

    protected void DecideOnNextAttack()
    {
        if (m_TimesFled >= 2)
        {
            m_CurrentComboIndex = BEFORE_DROP_GRENADE;
            return;
        }

        // cancelled before first attack -> just act as if it had happened
        if (m_CurrentComboIndex == -1)
        {
            m_CurrentComboIndex = 0;
        }

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
                m_CurrentComboIndex = ChooseNextAttackFrom(0, 1, 2, 3) - 1;
                return;
            }
            else // pistol / move-around-y atack
            {
                if (LastNAttacksWereTheSame(5))
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

    protected bool LastNAttacksWereTheSame(int n)
    {
        if (m_LastAttackTypes.Count < n)
            return false;

        for(int i = 1; i < n; i++)
        {
            if (m_LastAttackTypes[m_LastAttackTypes.Count - i] != m_LastAttackTypes[m_LastAttackTypes.Count - i - 1])
                return false;
        }

        // throwing a grenade doesn't really count, so even if the attacks were of the same type (pistol), if they only were that because there was a grenade,
        // that's still a "nope"

        return m_LastActualAttacks[m_LastActualAttacks.Count -1] != 2 && m_LastActualAttacks[m_LastActualAttacks.Count - 2] != 2 ;
    }

    protected int ChooseNextAttackFrom(params int[] possibilites)
    {
        int choice = possibilites[UnityEngine.Random.Range(0, possibilites.Length)];
        int maxTries = 5;

        if (m_LastActualAttacks.Count > 3)
        {
            for (int i = 0; i < maxTries; i++)
            {
                if (m_LastActualAttacks.GetRange(m_LastActualAttacks.Count - 4, 3).Contains(choice))
                    choice = possibilites[UnityEngine.Random.Range(0, possibilites.Length)];
                else
                    break;
            }
        }

        return choice;
    }

    protected override IEnumerator PrepareAttack(int attackIndex)
    {
        if (!m_NotDeactivated)
            yield break;

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

        if (attackIndex != 9)
        {
            yield return base.PrepareAttack(attackIndex);
        }
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
        if (!m_NotDeactivated)
            yield break;

        m_Evading = false;
        yield return null;
        MLog.Log(LogType.DHLog, "On Evasion Finished,  DH, " + this);

        DecideOnNextAttack();

        m_NextComboTimer = StartNextComboAfter(0.01f);
        StartCoroutine(m_NextComboTimer);
    }

    protected override IEnumerator AfterCombo(AttackCombo combo)
    {
        if (!m_NotDeactivated)
            yield break;

        m_RangeCheck = RangeCheckRoutine();
        StartCoroutine(m_RangeCheck);

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

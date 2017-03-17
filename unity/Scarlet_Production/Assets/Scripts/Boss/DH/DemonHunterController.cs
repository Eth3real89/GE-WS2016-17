using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DemonHunterController : BossController {

    protected static float[][] s_ThrowGrenadeSounds =
    {
        new float[] {86, 87.7f },
        new float[] {95f, 96.8f },
        new float[] {98.2f, 100.1f },
    };

    public enum AttackType {Pistols, Rifle, ThrowGrenade, DropGrenade };

    protected static string ANIM_AFTER_UNEQUIP_PISTOLS = "UnarmedIdle";
    protected static string ANIM_AFTER_EQUIP_PISTOLS = "PistolsIdle";
    protected static string ANIM_AFTER_RELOAD_PISTOLS = "PistolsIdle";

    protected static string ANIM_AFTER_UNEQUIP_RIFLE = "UnarmedIdle";
    protected static string ANIM_AFTER_EQUIP_RIFLE = "RifleIdle";
    protected static string ANIM_AFTER_RELOAD_RIFLE = "RifleIdle";

    protected static string ANIM_AFTER_THROW_GRENADE = "GrenadeThrown";

    public Animator m_DHAnimator;

    public BossfightCallbacks m_Callback;
    public float m_ReloadPistolTime;
    public float m_ReloadRifleTime;

    public DemonHunterWeaponChanges m_WeaponChanges;

    protected bool m_CanBeHit;
    protected bool m_ShootingPistols;

    public AttackType[] m_Types;

    public float m_MinHitWindowTime = 2f;
    public float m_DangerousDistance = 2f;
    protected IEnumerator m_RangeCheck;
    protected IEnumerator m_PreparationRoutine;

    protected IEnumerator m_ThrowGrenadeWhileRunningEnumerator;

    public DemonHunterEvasionCommand m_EvasionCommand;
    protected bool m_Evading;

    public Transform[] m_AttackSpots;
    protected bool[] m_BlockedByGrenade;

    protected bool m_SkipReload;
    protected bool m_SkipForceKeepWindowOpen;

    protected int m_TimesFled;

    public LightGuardAttack m_DropGrenadeAttack;

    public int m_NumHits = 10;
    public CharacterHealth m_DHHealth;

    public int m_StartAttackIndex = 0;

    public Transform m_PerfectRotationTarget;

    protected FancyAudioRandomClip m_ThrowGrenadePlayer;

    public virtual void StartPhase(BossfightCallbacks callback)
    {
        MLog.Log(LogType.DHLog, "Starting Phase, DH, " + this);
        m_Callback = callback;
        m_CanBeHit = false;
        m_ShootingPistols = false;
        m_SkipReload = false;
        m_BlockedByGrenade = new bool[m_AttackSpots.Length];
        m_Evading = false;
        m_TimesFled = 0;
        m_SkipForceKeepWindowOpen = false;
        m_PerfectRotationTarget = m_Scarlet.transform;
        m_ThrowGrenadePlayer = new FancyAudioRandomClip(s_ThrowGrenadeSounds, transform, "dh");

        ((DemonHunterHittable)m_BossHittable).m_NumHits = this.m_NumHits;

        for (int i = 0; i < m_BlockedByGrenade.Length; i++)
            m_BlockedByGrenade[i] = false;

        RegisterComboCallback();

        m_BossHittable.RegisterInterject(this);
        StartFirstCombo();
    }

    protected virtual void StartFirstCombo()
    {
        m_CurrentComboIndex = m_StartAttackIndex - 1;
        StartCoroutine(StartNextComboAfter(0.5f));
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        if (!m_NotDeactivated)
            yield break;

        yield return new WaitForSeconds(time);
        MLog.Log(LogType.DHLog, "Start Next Combo After, DH, " + this);

        if (m_RangeCheck != null)
            StopCoroutine(m_RangeCheck);

        if (m_PreparationRoutine != null)
            StopCoroutine(m_PreparationRoutine);

        // face scarlet, just looks weird otherwise
        m_PreparationRoutine = m_EvasionCommand.QuickPerfectRotationRoutine(0.2f, m_PerfectRotationTarget);
        yield return StartCoroutine(m_PreparationRoutine);

        if (m_TimesFled >= 2)
        {
            m_TimesFled = 0;
            MakeProtectiveGrenadeNext();
        }

        // if the next attack that is coming up allows for that: Allow the player to hit Scarlet!
        m_CanBeHit = MaybeOpenHitOpportunity();
        float timeKeeper = Time.timeSinceLevelLoad;

        int nextCombo = m_CurrentComboIndex + 1 >= m_Combos.Length ? 0 : m_CurrentComboIndex + 1;
        m_PreparationRoutine = PrepareAttack(nextCombo);
        yield return StartCoroutine(m_PreparationRoutine);

        if (Time.timeSinceLevelLoad - timeKeeper < m_MinHitWindowTime && !m_SkipForceKeepWindowOpen && MaybeOpenHitOpportunity())
        {
            yield return new WaitForSeconds(m_MinHitWindowTime - (Time.timeSinceLevelLoad - timeKeeper));
        }

        m_SkipForceKeepWindowOpen = false;

        if (ScarletTooClose(true) && m_CanBeHit)
        {
            m_CanBeHit = false;
            OnScarletTooClose(true);
        }
        else
        {
            m_CanBeHit = false;
            InitRangeCheck();
            StartNextCombo();
        }
    }

    protected bool MaybeOpenHitOpportunity()
    {
        int nextCombo = m_CurrentComboIndex + 1 >= m_Combos.Length ? 0 : m_CurrentComboIndex + 1;
        return m_Types[nextCombo] == AttackType.Pistols || m_Types[nextCombo] == AttackType.ThrowGrenade;
    }

    protected virtual void InitRangeCheck()
    {
        int nextCombo = m_CurrentComboIndex + 1 >= m_Combos.Length ? 0 : m_CurrentComboIndex + 1;
        if (InitRangeCheckCondition(nextCombo))
        {
            m_RangeCheck = RangeCheckRoutine();
            StartCoroutine(m_RangeCheck);
        }
    }

    protected virtual bool InitRangeCheckCondition(int nextCombo)
    {
        return m_Types[nextCombo] == AttackType.Pistols || m_Types[nextCombo] == AttackType.ThrowGrenade;
    }

    protected virtual IEnumerator PrepareAttack(int attackIndex)
    {
        MLog.Log(LogType.DHLog, "Preparing attack with index: " + attackIndex + ", DH, " + this);

        if (m_Types[attackIndex] == AttackType.Pistols)
        {
            m_PreparationRoutine = PreparePistols();
        }
        else if (m_Types[attackIndex] == AttackType.Rifle)
        {
            m_PreparationRoutine = PrepareRifle();
        }
        else if (m_Types[attackIndex] == AttackType.ThrowGrenade)
        {
            m_PreparationRoutine = PrepareGrenade(attackIndex);
        }
        else if (m_Types[attackIndex] == AttackType.DropGrenade)
        {
            m_PreparationRoutine = PrepareGrenadeDrop(attackIndex);
        }
        yield return StartCoroutine(m_PreparationRoutine);

        MLog.Log(LogType.DHLog, "Done preparing Attack, DH, " + attackIndex + " " + this);
    }

    protected IEnumerator PreparePistols()
    {
        MLog.Log(LogType.DHLog, "Preparing pistols, skipping reload: " + m_SkipReload + ", DH, " + this);

        if (m_WeaponChanges.m_CurrentlyEquipped != DemonHunterWeaponChanges.S_PISTOLS_EQUIPPED)
        {
            if (m_WeaponChanges.m_CurrentlyEquipped == DemonHunterWeaponChanges.S_RIFLE_EQUIPPED)
            {
                m_DHAnimator.SetTrigger("UnequipRifleTrigger");
                while (!CheckAnimation(ANIM_AFTER_UNEQUIP_RIFLE))
                    yield return null;
            }

            m_DHAnimator.SetTrigger("EquipPistolsTrigger");
            while (!CheckAnimation(ANIM_AFTER_EQUIP_PISTOLS))
                yield return null;
        }

        if (!m_SkipReload && !m_DropGrenadeAttack.IsActive())
        {
            m_DHAnimator.SetTrigger("ReloadTrigger");

            yield return new WaitForSeconds(0.2f);
            while (!CheckAnimation(ANIM_AFTER_RELOAD_PISTOLS))
                yield return null;
        }
        else
        {
            m_DHAnimator.ResetTrigger("ReloadTrigger");
            m_SkipReload = false;
        }

        m_PreparationRoutine = m_EvasionCommand.QuickPerfectRotationRoutine(0.2f, m_PerfectRotationTarget);
        yield return m_PreparationRoutine;

        MLog.Log(LogType.DHLog, "Done preparing pistols, DH, " + this);
    }

    protected IEnumerator PrepareRifle()
    {
        MLog.Log(LogType.DHLog, "Preparing Rifle, skipping reload: " + m_SkipReload + ", DH, " + this);
        if (m_WeaponChanges.m_CurrentlyEquipped != DemonHunterWeaponChanges.S_RIFLE_EQUIPPED)
        {
            if (m_WeaponChanges.m_CurrentlyEquipped == DemonHunterWeaponChanges.S_PISTOLS_EQUIPPED)
            {
                m_DHAnimator.SetTrigger("UnequipPistolsTrigger");
                while (!CheckAnimation(ANIM_AFTER_UNEQUIP_PISTOLS))
                    yield return null;
            }

            m_DHAnimator.SetTrigger("EquipRifleTrigger");
            while (!CheckAnimation(ANIM_AFTER_EQUIP_RIFLE))
                yield return null;
        }


        //if (!m_SkipReload)
        //{
            m_DHAnimator.SetTrigger("ReloadTrigger");
            yield return new WaitForSeconds(0.2f);
            while (!CheckAnimation(ANIM_AFTER_RELOAD_RIFLE))
                yield return null;
        /*}
        else
        {
            m_SkipReload = false;
        }*/

        m_PreparationRoutine = m_EvasionCommand.QuickPerfectRotationRoutine(0.2f, m_PerfectRotationTarget);
        yield return m_PreparationRoutine;
        m_SkipReload = false;

        MLog.Log(LogType.DHLog, "Done preparing Rifle, DH, " + this);
    }

    protected IEnumerator PrepareGrenade(int attackIndex)
    {
        if (m_WeaponChanges.m_CurrentlyEquipped == DemonHunterWeaponChanges.S_NOTHING_EQUIPPED)
        {
            m_DHAnimator.SetTrigger("EquipPistolsTrigger");
            while (!CheckAnimation(ANIM_AFTER_EQUIP_PISTOLS))
                yield return null;
        }

        m_DHAnimator.SetTrigger("ThrowGrenadeTrigger");
        yield return new WaitForSeconds(0.1f);

        if (m_WeaponChanges.m_CurrentlyEquipped == DemonHunterWeaponChanges.S_PISTOLS_EQUIPPED)
        {
            while (!CheckAnimation(ANIM_AFTER_THROW_GRENADE))
                yield return null;
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }
        
        Transform t = ChooseGrenadeSpot(m_Combos[attackIndex]);

        m_PreparationRoutine = m_EvasionCommand.QuickPerfectRotationRoutine(0.2f, t);
        yield return m_PreparationRoutine;

        m_SkipReload = false;
    }

    protected IEnumerator PrepareGrenadeDrop(int attackIndex)
    {
        m_TimesFled = 0;

        if (m_WeaponChanges.m_CurrentlyEquipped != DemonHunterWeaponChanges.S_PISTOLS_EQUIPPED)
        {
            if (m_WeaponChanges.m_CurrentlyEquipped == DemonHunterWeaponChanges.S_RIFLE_EQUIPPED)
            {
                m_DHAnimator.SetTrigger("UnequipRifleTrigger");
                while (!CheckAnimation(ANIM_AFTER_UNEQUIP_RIFLE))
                    yield return null;
            }

            m_DHAnimator.SetTrigger("EquipPistolsTrigger");
            while (!CheckAnimation(ANIM_AFTER_EQUIP_PISTOLS))
                yield return null;
        }

        m_DHAnimator.SetTrigger("DropGrenadeTrigger");
        yield return new WaitForSeconds(0.5f);
        m_SkipReload = false;
    }

    protected virtual IEnumerator RangeCheckRoutine(bool triggerSkipReload = true)
    {
        while(true)
        {
            if (ScarletTooClose())
            {
                OnScarletTooClose(triggerSkipReload);
            }

            yield return null;
        }
    }

    protected virtual bool ScarletTooClose(bool onlyCheckRange = false)
    {
        float dist = Vector3.Distance(transform.position, m_Scarlet.transform.position);
        return dist <= m_DangerousDistance && ((onlyCheckRange) || (!m_DropGrenadeAttack.IsActive() && !m_Evading && !m_CanBeHit));
    }

    protected void MakeProtectiveGrenadeNext()
    {
        m_CurrentComboIndex = 3;
    }

    protected virtual void OnScarletTooClose(bool skipReload)
    {
        MLog.Log(LogType.DHLog, "On Scarlet Too Close,  DH, " + this);

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        if (m_RangeCheck != null)
            StopCoroutine(m_RangeCheck);

        if (m_PreparationRoutine != null)
            StopCoroutine(m_PreparationRoutine);

        CancelComboIfActive();

        m_SkipReload = skipReload;
        m_SkipForceKeepWindowOpen = skipReload;
        m_TimesFled++;

        StartEvasion(true);
    }

    protected virtual Transform GetFurthestSpotFromScarlet()
    {
        Transform maxDistTransform = null;
        float maxDist = -1;

        for(int i = 0; i < m_AttackSpots.Length; i++)
        {
            Transform t = m_AttackSpots[i];
            float dist = Vector3.Distance(t.position, m_Scarlet.transform.position);

            if (dist > maxDist && !m_BlockedByGrenade[i] && Vector3.Distance(transform.position, t.position) > 2)
            {
                maxDistTransform = t;
                maxDist = dist;
            }
        }

        return maxDistTransform;
    }

    protected virtual IEnumerator OnEvasionFinished()
    {
        if (!m_NotDeactivated)
            yield break;

        m_Evading = false;
        yield return null;
        MLog.Log(LogType.DHLog, "On Evasion Finished,  DH, " + this);

        m_NextComboTimer = StartNextComboAfter(0.01f);
        StartCoroutine(m_NextComboTimer);
    }

    protected virtual IEnumerator OnGrenadeThrowingEvasionFinished()
    {
        if (!m_NotDeactivated)
            yield break;

        m_Evading = false;
        yield return null;
        MLog.Log(LogType.DHLog, "On Grenade Throwing Evasion Finished,  DH, " + this);
    }

    protected virtual bool CheckAnimation(string animation)
    {
        return !m_DHAnimator.IsInTransition(0) && m_DHAnimator.GetCurrentAnimatorStateInfo(0).IsName(animation);
    }

    public override void OnComboStart(AttackCombo combo)
    {
        MLog.Log(LogType.DHLog, "On Combo Start,  DH, " + combo + " " + this);
        base.OnComboStart(combo);
    }

    protected virtual Transform ChooseGrenadeSpot(AttackCombo attackCombo)
    {
        Transform furthestFromBossThatIsntBlocked = null;
        float maxDist = -1;
        int spotIndex = -1;

        for(int i = 0; i < m_AttackSpots.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, m_AttackSpots[i].position);
            if (dist > maxDist && !m_BlockedByGrenade[i])
            {
                maxDist = dist;
                spotIndex = i;
                furthestFromBossThatIsntBlocked = m_AttackSpots[i];
            }
        }

        for(int i = 0; i < attackCombo.m_Attacks.Length; i++)
        {
            if (attackCombo.m_Attacks[i] is BulletAttack)
            {
                BulletMovement m = (((BulletAttack)attackCombo.m_Attacks[i]).m_BaseSwarm.m_Invoker.m_Factories[0].m_Movement);
                if (m is BulletGrenadeMovement)
                {
                    ((BulletGrenadeMovement)m).m_Goal = furthestFromBossThatIsntBlocked;
                    break;
                }
            }
        }

        StartCoroutine(BlockAttackSpot(spotIndex, 15f));
        MLog.Log(LogType.DHLog, "Decided on Grenade Goal, DH, " + furthestFromBossThatIsntBlocked + " " + this);

        return furthestFromBossThatIsntBlocked;
    }

    protected virtual IEnumerator BlockAttackSpot(int spotIndex, float howLong)
    {
        m_BlockedByGrenade[spotIndex] = true;
        yield return new WaitForSeconds(howLong);
        m_BlockedByGrenade[spotIndex] = false;
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        if (!m_NotDeactivated)
            return;

        MLog.Log(LogType.DHLog, "On Combo End,  DH, " + combo + " " + this);

        if (m_RangeCheck != null)
            StopCoroutine(m_RangeCheck);
        
        if (m_PreparationRoutine != null)
            StopCoroutine(m_PreparationRoutine);

        if (m_ThrowGrenadeWhileRunningEnumerator != null)
            StopCoroutine(m_ThrowGrenadeWhileRunningEnumerator);

        m_ShootingPistols = false;
        m_ActiveCombo = null;
        m_ComboActive = false;

        m_BossHittable.RegisterInterject(this);
        m_BlockingBehaviour.m_TimesBlockBeforeParry = m_MaxBlocksBeforeParry;
        SetDropGrenadeAttackGoal(m_BossHittable.transform, true);

        m_NextComboTimer = AfterCombo(combo);
        StartCoroutine(m_NextComboTimer);
    }

    protected virtual IEnumerator AfterCombo(AttackCombo combo)
    {
        if (!m_NotDeactivated)
            yield break;

        if (m_CurrentComboIndex > 1 && m_Types[m_CurrentComboIndex - 1] == AttackType.DropGrenade)
        {
            m_DropGrenadeAttack.CancelAttack();
        }
        
        m_RangeCheck = RangeCheckRoutine(false);
        StartCoroutine(m_RangeCheck);

        yield return new WaitForSeconds(combo.m_TimeAfterCombo);

        if (combo != m_Combos[m_CurrentComboIndex])
            yield break;

        if (m_RangeCheck != null)
            StopCoroutine(m_RangeCheck);

        if (m_PreparationRoutine != null)
            StopCoroutine(m_PreparationRoutine);

        if (m_ThrowGrenadeWhileRunningEnumerator != null)
            StopCoroutine(m_ThrowGrenadeWhileRunningEnumerator);

        InitNextAttack();
    }

    protected virtual void InitNextAttack()
    {
        if (m_Types[m_CurrentComboIndex] == AttackType.DropGrenade)
        {
            MLog.Log(LogType.DHLog, "Starting next combo from AfterCombo, DH, After Grenade, " + this);
            m_NextComboTimer = StartNextComboAfter(0.01f);
            StartCoroutine(m_NextComboTimer);
        }
        else
        {
            MLog.Log(LogType.DHLog, "Starting next combo from AfterCombo, DH, After Evasion, " + this);
            StartEvasion(false);
        }
    }

    public override void CancelComboIfActive()
    {
        base.CancelComboIfActive();
        if (m_ThrowGrenadeWhileRunningEnumerator != null)
            StopCoroutine(m_ThrowGrenadeWhileRunningEnumerator);
    }

    protected virtual void StartEvasion(bool m_PossiblyThrowWhileRunning)
    {
        Transform target = GetFurthestSpotFromScarlet();

        if (m_TimesFled >= 2)
        {
            m_TimesFled = 0;
            MakeProtectiveGrenadeNext();
        }
        m_Evading = true;

        int nextCombo = m_CurrentComboIndex + 1 >= m_Combos.Length ? 0 : m_CurrentComboIndex + 1;
        if (m_Types[nextCombo] == AttackType.DropGrenade && m_PossiblyThrowWhileRunning)
        {
            SetDropGrenadeAttackGoal(target, false);
            m_EvasionCommand.EvadeTowards(target, this, OnGrenadeThrowingEvasionFinished());
            PlayGrenadeSound();
            m_ThrowGrenadeWhileRunningEnumerator = ThrowGrenadeWhileRunning();
            StartCoroutine(m_ThrowGrenadeWhileRunningEnumerator);
        }
        else
        {
            m_EvasionCommand.EvadeTowards(target, this, OnEvasionFinished());
        }
    }

    protected virtual IEnumerator ThrowGrenadeWhileRunning()
    {
        yield return new WaitForSeconds(0.5f);

        StartNextCombo();
    }

    protected virtual void SetDropGrenadeAttackGoal(Transform t, bool atFeet)
    {
        try
        {
            BulletAttack grenadeAttack = (BulletAttack) m_Combos[4].m_Attacks[0];
            BulletGrenadeMovement movement = (BulletGrenadeMovement) grenadeAttack.m_BaseSwarm.m_Invoker.m_Factories[0].m_Movement;
            movement.m_Goal = t;

            try
            {
                if (atFeet)
                {
                    movement.m_TimeToReachGoal = 0.05f;
                    BulletTimeBasedExpiration expire = (BulletTimeBasedExpiration)grenadeAttack.m_BaseSwarm.m_Invoker.m_Factories[0].m_Expire;
                    expire.m_Time = 0.06f;
                }
                else
                {
                    movement.m_TimeToReachGoal = 0.6f;
                    BulletTimeBasedExpiration expire = (BulletTimeBasedExpiration)grenadeAttack.m_BaseSwarm.m_Invoker.m_Factories[0].m_Expire;
                    expire.m_Time = 0.61f;
                }
            } catch { }

        }
        catch { };
    }

    public override bool OnHit(Damage dmg)
    {
        MLog.Log(LogType.DHLog, "Something hit DH, " + this);

        if (dmg is BulletDamage)
        {
            return false;
        }

        if (m_CanBeHit)
        {
            MLog.Log(LogType.DHLog, "Successful hit, DH, " + this);

            CameraController.Instance.Shake();
            CancelComboIfActive();
            if (m_RangeCheck != null)
                StopCoroutine(m_RangeCheck);

            if (m_PreparationRoutine != null)
                StopCoroutine(m_PreparationRoutine);

            if (m_ThrowGrenadeWhileRunningEnumerator != null)
                StopCoroutine(m_ThrowGrenadeWhileRunningEnumerator);

            dmg.OnSuccessfulHit();
            CameraController.Instance.Shake();

            m_DHAnimator.ResetTrigger("ReloadTrigger");

            m_CanBeHit = false;
            m_SkipReload = true;
            m_SkipForceKeepWindowOpen = true;
            
            m_CurrentComboIndex++;

            StartEvasion(true);

            return false;
        }
        else
        {
            return true;
        }
    }

    protected virtual void PlayGrenadeSound()
    {
        m_ThrowGrenadePlayer.PlayRandomSound();
    }
}

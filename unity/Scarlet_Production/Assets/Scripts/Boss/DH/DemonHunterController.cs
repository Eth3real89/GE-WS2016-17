using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DemonHunterController : BossController {

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

    protected bool m_Reloading;
    protected bool m_ShootingPistols;

    public AttackType[] m_Types;

    public float m_DangerousDistance = 2f;
    protected IEnumerator m_RangeCheck;
    protected IEnumerator m_PreparationRoutine;

    public DemonHunterEvasionCommand m_EvasionCommand;
    protected bool m_Evading;

    public Transform[] m_AttackSpots;
    protected bool[] m_BlockedByGrenade;

    protected bool m_SkipReload;

    protected int m_TimesFled;

    public LightGuardAttack m_DropGrenadeAttack;

    public int m_NumHits = 10;
    public CharacterHealth m_DHHealth;

    public int m_StartAttackIndex = 0;

    public Transform m_PerfectRotationTarget;

    public virtual void StartPhase(BossfightCallbacks callback)
    {
        MLog.Log(LogType.DHLog, "Starting Phase, DH, " + this);
        this.m_Callback = callback;
        m_Reloading = false;
        m_ShootingPistols = false;
        m_SkipReload = false;
        m_BlockedByGrenade = new bool[m_AttackSpots.Length];
        m_Evading = false;
        m_TimesFled = 0;
        m_PerfectRotationTarget = m_Scarlet.transform;

        ((DemonHunterHittable)m_BossHittable).m_NumHits = this.m_NumHits;

        for (int i = 0; i < m_BlockedByGrenade.Length; i++)
            m_BlockedByGrenade[i] = false;
    
        RegisterComboCallback();

        m_BossHittable.RegisterInterject(this);

        m_CurrentComboIndex = m_StartAttackIndex -1;
        StartCoroutine(StartNextComboAfter(0.5f));
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        yield return new WaitForSeconds(time);
        MLog.Log(LogType.DHLog, "Start Next Combo After, DH, " + this);

        if (m_RangeCheck != null)
            StopCoroutine(m_RangeCheck);

        if (m_PreparationRoutine != null)
            StopCoroutine(m_PreparationRoutine);

        m_PreparationRoutine = m_EvasionCommand.QuickPerfectRotationRoutine(0.2f, m_PerfectRotationTarget);
        yield return StartCoroutine(m_PreparationRoutine);

        if (m_TimesFled >= 2)
        {
            m_TimesFled = 0;
            MakeProtectiveGrenadeNext();
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

        if (!m_SkipReload)
        {
            m_Reloading = true;
            m_DHAnimator.SetTrigger("ReloadTrigger");

            yield return new WaitForSeconds(0.2f);
            while (!CheckAnimation(ANIM_AFTER_RELOAD_PISTOLS))
                yield return null;
            m_Reloading = false;
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

        m_DHAnimator.SetTrigger("ReloadTrigger");
        yield return new WaitForSeconds(0.2f);
        while (!CheckAnimation(ANIM_AFTER_RELOAD_RIFLE))
            yield return null;

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
            yield return new WaitForSeconds(0.35f);
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

    protected virtual IEnumerator RangeCheck()
    {
        while(true)
        {
            float dist = Vector3.Distance(transform.position, m_Scarlet.transform.position);
            if (dist <= m_DangerousDistance && !m_DropGrenadeAttack.IsActive() && !m_Evading)
            {
                OnScarletTooClose();
            }

            yield return null;
        }
    }

    protected void MakeProtectiveGrenadeNext()
    {
        m_CurrentComboIndex = 3;
    }

    protected virtual void OnScarletTooClose()
    {
        MLog.Log(LogType.DHLog, "On Scarlet Too Close,  DH, " + this);

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        if (m_RangeCheck != null)
            StopCoroutine(m_RangeCheck);

        if (m_PreparationRoutine != null)
            StopCoroutine(m_PreparationRoutine);

        CancelComboIfActive();

        m_SkipReload = true;
        m_TimesFled++;

        m_Evading = true;
        m_EvasionCommand.EvadeTowards(GetFurthestSpotFromScarlet(), this, OnEvasionFinished());
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
        m_Evading = false;
        yield return null;
        MLog.Log(LogType.DHLog, "On Evasion Finished,  DH, " + this);

        m_NextComboTimer = StartNextComboAfter(0.01f);
        StartCoroutine(m_NextComboTimer);
    }

    protected virtual bool CheckAnimation(string animation)
    {
        return !m_DHAnimator.IsInTransition(0) && m_DHAnimator.GetCurrentAnimatorStateInfo(0).IsName(animation);
    }

    public override void OnComboStart(AttackCombo combo)
    {
        MLog.Log(LogType.DHLog, "On Combo Start,  DH, " + combo + " " + this);
        base.OnComboStart(combo);

        if (m_Types[m_CurrentComboIndex] == AttackType.Pistols)
        {
            m_RangeCheck = RangeCheck();
            StartCoroutine(m_RangeCheck);
        }
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
        MLog.Log(LogType.DHLog, "On Combo End,  DH, " + combo + " " + this);

        if (m_RangeCheck != null)
            StopCoroutine(m_RangeCheck);
        
        if (m_PreparationRoutine != null)
            StopCoroutine(m_PreparationRoutine);

        m_ShootingPistols = false;
        m_ActiveCombo = null;
        m_ComboActive = false;

        m_BossHittable.RegisterInterject(this);
        m_BlockingBehaviour.m_TimesBlockBeforeParry = m_MaxBlocksBeforeParry;

        m_NextComboTimer = AfterCombo(combo);
        StartCoroutine(m_NextComboTimer);
    }

    protected virtual IEnumerator AfterCombo(AttackCombo combo)
    {
        if (m_CurrentComboIndex > 1 && m_Types[m_CurrentComboIndex - 1] == AttackType.DropGrenade)
        {
            m_DropGrenadeAttack.CancelAttack();
        }

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
            m_Evading = true;
            m_EvasionCommand.EvadeTowards(GetFurthestSpotFromScarlet(), this, OnEvasionFinished());
        }
    }

    public override bool OnHit(Damage dmg)
    {
        if (dmg is BulletDamage)
        {
            return false;
        }

        if (m_Reloading)
        {
            MLog.Log(LogType.DHLog, "Successful hit, DH, " + this);

            CancelComboIfActive();
            if (m_RangeCheck != null)
                StopCoroutine(m_RangeCheck);

            if (m_PreparationRoutine != null)
                StopCoroutine(m_PreparationRoutine);

            dmg.OnSuccessfulHit();
            CameraController.Instance.Shake();

            m_DHAnimator.ResetTrigger("ReloadTrigger");

            m_Reloading = false;
            m_SkipReload = true;
            MLog.Log(LogType.DHLog, 1, "m_SkipReload, DH, " + m_SkipReload + " " + this);

            m_CurrentComboIndex++;

            m_Evading = true;
            m_EvasionCommand.EvadeTowards(GetFurthestSpotFromScarlet(), this, OnEvasionFinished());

            return false;
        }
        else
        {
            return true;
        }
    }

}

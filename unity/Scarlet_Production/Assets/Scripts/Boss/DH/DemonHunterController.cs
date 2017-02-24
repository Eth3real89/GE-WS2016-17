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

    public DemonHunterEvasionCommand m_EvasionCommand;

    public Transform[] m_AttackSpots;
    protected bool[] m_BlockedByGrenade;

    protected bool m_SkipReload;

    protected bool m_RunAwayInstead;
    protected int m_TimesFled;

    public LightGuardAttack m_DropGrenadeAttack;

    public int m_NumHits = 10;

    public int m_StartAttackIndex = 0;

    public virtual void StartPhase(BossfightCallbacks callback)
    {
        this.m_Callback = callback;
        m_Reloading = false;
        m_ShootingPistols = false;
        m_SkipReload = false;
        m_RunAwayInstead = false;
        m_BlockedByGrenade = new bool[m_AttackSpots.Length];
        m_TimesFled = 0;
        ((DemonHunterHittable)m_BossHittable).m_NumHits = this.m_NumHits;


        for (int i = 0; i < m_BlockedByGrenade.Length; i++)
            m_BlockedByGrenade[i] = false;
    

        RegisterComboCallback();

        m_BossHittable.RegisterInterject(this);

        m_CurrentComboIndex = m_StartAttackIndex -1;
        StartCoroutine(StartNextComboAfter(0.5f));
    }

    protected virtual void RegisterListeners()
    {

    }

    protected virtual void UnregisterListeners()
    {

    }

    protected override void StartNextCombo()
    {
        if (m_RunAwayInstead)
        {
            m_NextComboTimer = RunToNextSpot();
            StartCoroutine(m_NextComboTimer);
            m_RunAwayInstead = false;
        }
        else
        {
            base.StartNextCombo();
        }
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        yield return new WaitForSeconds(time);

        if (m_RangeCheck != null)
            StopCoroutine(m_RangeCheck);

        yield return m_EvasionCommand.QuickPerfectRotationRoutine(0.2f);

        if (m_TimesFled >= 2)
        {
            m_TimesFled = 0;
            yield return DropProtectiveGrenade();
        }

        if (m_CurrentComboIndex + 1 >= m_Combos.Length)
        {
            yield return PrepareAttack(0);
        }
        else
        {
            yield return PrepareAttack(m_CurrentComboIndex + 1);
        }
        
        StartNextCombo();
    }

    protected virtual IEnumerator PrepareAttack(int attackIndex)
    {
        if (m_Types[attackIndex] == AttackType.Pistols)
        {
            yield return PreparePistols();
            yield return m_EvasionCommand.QuickPerfectRotationRoutine(0.2f);
        }
        else if (m_Types[attackIndex] == AttackType.Rifle)
        {
            yield return PrepareRifle();
            yield return m_EvasionCommand.QuickPerfectRotationRoutine(0.2f);
        }
        else if (m_Types[attackIndex] == AttackType.ThrowGrenade)
        {
            yield return PrepareGrenade(attackIndex);
        }
        else if (m_Types[attackIndex] == AttackType.DropGrenade)
        {
            yield return PrepareGrenadeDrop(attackIndex);
        }
    }

    protected IEnumerator PreparePistols()
    {
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
        m_SkipReload = false;
    }

    protected IEnumerator PrepareRifle()
    {
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
    }

    protected IEnumerator PrepareGrenade(int attackIndex)
    {
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

        m_DHAnimator.SetTrigger("ThrowGrenadeTrigger");
        yield return new WaitForSeconds(0.1f);
        while (!CheckAnimation(ANIM_AFTER_THROW_GRENADE))
            yield return null;
        
        Transform t = ChooseGrenadeSpot(m_Combos[attackIndex]);
        yield return m_EvasionCommand.QuickPerfectRotationRoutine(0.2f, t);
    }

    protected IEnumerator PrepareGrenadeDrop(int attackIndex)
    {
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
    }

    protected virtual IEnumerator RunToNextSpot()
    {
        yield return m_EvasionCommand.ReachSpot(GetFurthestSpotFromScarlet());
        m_NextComboTimer = StartNextComboAfter(0.1f);
        StartCoroutine(m_NextComboTimer);
    }

    protected virtual IEnumerator ReloadPistols(IEnumerator then)
    {
        m_Reloading = true;
        yield return new WaitForSeconds(m_ReloadPistolTime);
        m_Reloading = false;

        m_NextComboTimer = then;
        StartCoroutine(m_NextComboTimer);
    }

    protected virtual IEnumerator ReloadRifle(IEnumerator then)
    {
        m_Reloading = true;
        yield return new WaitForSeconds(m_ReloadRifleTime);
        m_Reloading = false;

        m_NextComboTimer = then;
        StartCoroutine(m_NextComboTimer);
    }

    protected virtual IEnumerator RangeCheck()
    {
        while(true)
        {
            float dist = Vector3.Distance(transform.position, m_Scarlet.transform.position);
            if (dist <= m_DangerousDistance && !m_DropGrenadeAttack.IsActive())
            {
                OnScarletTooClose();
            }

            yield return null;
        }
    }

    protected IEnumerator DropProtectiveGrenade()
    {
        m_CurrentComboIndex = 3;
        yield return null;
    }

    protected virtual void OnScarletTooClose()
    {
        if (m_RangeCheck != null)
            StopCoroutine(m_RangeCheck);
        CancelComboIfActive();

        m_SkipReload = true;
        m_TimesFled++;

        m_EvasionCommand.EvadeTowards(GetFurthestSpotFromScarlet(), this, OnEvasionFinished());
    }

    private Transform GetFurthestSpotFromScarlet()
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
        yield return null;
        m_NextComboTimer = StartNextComboAfter(0.01f);
        StartCoroutine(m_NextComboTimer);
    }

    protected virtual bool CheckAnimation(string animation)
    {
        return !m_DHAnimator.IsInTransition(0) && m_DHAnimator.GetCurrentAnimatorStateInfo(0).IsName(animation);
    }

    public override void OnComboStart(AttackCombo combo)
    {
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
        if (m_RangeCheck != null)
            StopCoroutine(m_RangeCheck);
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

        yield return new WaitForSeconds(combo.m_TimeAfterCombo);

        if (m_Types[m_CurrentComboIndex] == AttackType.DropGrenade)
        {
            m_NextComboTimer = StartNextComboAfter(combo.m_TimeAfterCombo);
            StartCoroutine(m_NextComboTimer);
        }
        else
        {
            m_EvasionCommand.EvadeTowards(GetFurthestSpotFromScarlet(), this, OnEvasionFinished());
        }
    }

    public override void OnBossStaggerOver()
    {
        m_RunAwayInstead = true;
        base.OnBossStaggerOver();
    }

    public override void OnTimeWindowClosed()
    {
        m_RunAwayInstead = true;
        base.OnTimeWindowClosed();
    }

    public override bool OnHit(Damage dmg)
    {
        if (m_Reloading)
        { 
            CancelComboIfActive();
            if (m_RangeCheck != null)
                StopCoroutine(m_RangeCheck);

            dmg.OnSuccessfulHit();
            CameraController.Instance.Shake();

            m_Reloading = false;

            m_EvasionCommand.EvadeTowards(GetFurthestSpotFromScarlet(), this, OnEvasionFinished());

            return false;
        }
        else
        {
            return true;
        }
    }

}

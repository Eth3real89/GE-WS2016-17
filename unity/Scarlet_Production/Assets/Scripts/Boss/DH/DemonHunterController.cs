﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DemonHunterController : BossController {

    public enum AttackType {Pistols, Rifle, Grenade };

    protected static string ANIM_AFTER_UNEQUIP_PISTOLS = "UnarmedIdle";
    protected static string ANIM_AFTER_EQUIP_PISTOLS = "PistolsIdle";
    protected static string ANIM_AFTER_RELOAD_PISTOLS = "PistolsIdle";

    protected static string ANIM_AFTER_UNEQUIP_RIFLE = "UnarmedIdle";
    protected static string ANIM_AFTER_EQUIP_RIFLE = "RifleIdle";
    protected static string ANIM_AFTER_RELOAD_RIFLE = "RifleIdle";

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

    protected bool m_SkipReload;

    protected bool m_RunAwayInstead;

    public virtual void StartPhase(BossfightCallbacks callback)
    {
        this.m_Callback = callback;
        m_Reloading = false;
        m_ShootingPistols = false;
        m_SkipReload = false;
        m_RunAwayInstead = false;

        RegisterComboCallback();

        m_BossHittable.RegisterInterject(this);

        m_CurrentComboIndex = -1;
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

        if (m_CurrentComboIndex + 1 >= m_Combos.Length)
        {
            yield return PrepareAttack(0);
        }
        else
        {
            yield return PrepareAttack(m_CurrentComboIndex + 1);
        }

        yield return m_EvasionCommand.QuickPerfectRotationRoutine(0.2f);
        StartNextCombo();
    }

    protected virtual IEnumerator PrepareAttack(int attackIndex)
    {
        if (m_Types[attackIndex] == AttackType.Pistols)
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
        else if (m_Types[attackIndex] == AttackType.Rifle)
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
            if (dist <= m_DangerousDistance)
            {
                OnScarletTooClose();
            }

            yield return null;
        }
    }

    protected virtual void OnScarletTooClose()
    {
        StopCoroutine(m_RangeCheck);
        CancelComboIfActive();

        m_SkipReload = true;

        m_EvasionCommand.EvadeTowards(GetFurthestSpotFromScarlet(), this, OnEvasionFinished());
    }

    private Transform GetFurthestSpotFromScarlet()
    {
        Transform maxDistTransform = null;
        float maxDist = -1;

        foreach(Transform t in m_AttackSpots)
        {
            float dist = Vector3.Distance(t.position, m_Scarlet.transform.position);
            if (dist > maxDist)
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

    public override void OnComboEnd(AttackCombo combo)
    {
        StopCoroutine(m_RangeCheck);
        CancelComboIfActive();
        m_ShootingPistols = false;

        base.OnComboEnd(combo);
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

            if (m_TimeWindowManager != null)
            {
                m_TimeWindowManager.Activate(this);
                m_BossHittable.RegisterInterject(m_TimeWindowManager);
            }

            CameraController.Instance.Shake();

            return false;
        }
        else
        {
            return true;
        }
    }

}

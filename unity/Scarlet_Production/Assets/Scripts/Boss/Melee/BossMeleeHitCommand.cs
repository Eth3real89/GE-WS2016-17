using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMeleeHitCommand : BossCommand, DamageCollisionHandler {

    public BossMeleeDamage m_DamageTrigger;

    public GameObject[] m_HitSignals;

    private IEnumerator m_Timer;
    private IEnumerator m_DamageTimer;

    public float m_UpswingTime = 0.1f;
    public float m_HoldTime = 0.15f;
    public float m_DownswingTime = 0.3f;
    public float m_TimeBeforeDownswingCausesDamage = 0.2f;

    public AudioSource m_HitAudio;

    private MeleeHitCallback m_Callback;
    private MeleeHitStepsCallback m_StepsCallback;
    private int m_CurrentAnimation;

    public void DoHit(MeleeHitCallback callback, MeleeHitStepsCallback stepsCallback = null, int whichAnimation = 0)
    {
        m_Callback = callback;
        m_StepsCallback = stepsCallback;
        m_CurrentAnimation = whichAnimation;
        Upswing();   
    }

    public void Upswing()
    {
        m_Animator.SetTrigger("MeleeUpswingTrigger");
        m_Animator.SetInteger("WhichAttackAnimation", m_CurrentAnimation);
        m_Timer = SignalDownswingAfter(m_UpswingTime);
        StartCoroutine(m_Timer);

        if (m_StepsCallback != null)
            m_StepsCallback.OnMeleeUpswingStart();
    }

    private void SignalDownswing()
    {
        m_Timer = DownswingAfter(m_HoldTime);
        StartCoroutine(m_Timer);
        SetHitSignalState(true);

        if (m_StepsCallback != null)
            m_StepsCallback.OnMeleeHalt();
    }

    private void SetHitSignalState(bool active)
    {
        if (m_HitSignals != null && m_HitSignals.Length > m_CurrentAnimation && m_HitSignals[m_CurrentAnimation] != null)
        {
            m_HitSignals[m_CurrentAnimation].SetActive(active);
        }
    }

    public void Downswing(int whichAnimation = 0)
    {
        SetHitSignalState(false);

        if (m_HitAudio != null)
            m_HitAudio.Play();

        m_Animator.SetTrigger("MeleeDownswingTrigger");
        m_DamageTimer = EnableDamageAfter(m_TimeBeforeDownswingCausesDamage);
        StartCoroutine(m_DamageTimer);

        m_Timer = EndAfter(m_DownswingTime);
        StartCoroutine(m_Timer);

        if (m_StepsCallback != null)
            m_StepsCallback.OnMeleeDownswingStart();
    }

    public void EndAttack()
    {
        m_DamageTrigger.m_Active = false;

        if (m_StepsCallback != null)
            m_StepsCallback.OnMeleeEnd();
    }

    private IEnumerator SignalDownswingAfter(float time)
    {
        yield return new WaitForSeconds(time);
        SignalDownswing();
    }

    private IEnumerator DownswingAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Downswing();
    }

    private IEnumerator EndAfter(float time)
    {
        yield return new WaitForSeconds(time);
        m_DamageTrigger.m_Active = false;

        if (m_DamageTimer != null)
            StopCoroutine(m_DamageTimer);

        m_Callback.OnMeleeHitEnd();

        if (m_StepsCallback != null)
            m_StepsCallback.OnMeleeEnd();
    }

    private IEnumerator EnableDamageAfter(float time)
    {
        yield return new WaitForSeconds(time);
        m_DamageTrigger.m_CollisionHandler = this;
        m_DamageTrigger.m_Active = true;
    }

    public void CancelHit()
    {
        if (m_Timer != null)
        {
            StopCoroutine(m_Timer);
            m_DamageTrigger.m_Active = false;
        }

        if (m_DamageTimer != null)
            StopCoroutine(m_DamageTimer);

    }

    public void HandleScarletCollision(Collider other)
    {
        Hittable hittable = other.GetComponentInChildren<Hittable>();
        if (hittable != null && hittable is PlayerHittable)
        {
            if (m_DamageTrigger.m_Active)
            {
                hittable.Hit(m_DamageTrigger);
                m_Callback.OnMeleeHitSuccess();
                m_DamageTrigger.m_Active = false;
            }
        }
    }

    public void HandleCollision(Collider other, bool initialCollision)
    {
    }

    public void HandleScarletLeave(Collider other)
    {
    }

    public interface MeleeHitCallback
    {
        void OnMeleeHitSuccess();
        void OnMeleeHitEnd();
    }


    public interface MeleeHitStepsCallback
    {
        void OnMeleeDownswingStart();
        void OnMeleeHalt();
        void OnMeleeUpswingStart();
        void OnMeleeEnd();
    }

}

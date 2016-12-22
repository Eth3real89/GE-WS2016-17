using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMeleeHitCommand : BossCommand, DamageCollisionHandler {

    public BossMeleeDamage m_DamageTrigger;

    public GameObject m_HitSignal;

    private IEnumerator m_Timer;

    public float m_UpswingTime = 0.1f;
    public float m_HoldTime = 0.15f;
    public float m_DownswingTime = 0.3f;

    private MeleeHitCallback m_Callback;

    public void DoHit(MeleeHitCallback callback)
    {
        m_Callback = callback;
        Upswing();   
    }

    public void Upswing()
    {
        m_Animator.SetTrigger("MeleeUpswingTrigger");
        m_Timer = SignalDownswingAfter(m_UpswingTime);
        StartCoroutine(m_Timer);
    }

    private void SignalDownswing()
    {
        m_Timer = DownswingAfter(m_HoldTime);
        StartCoroutine(m_Timer);

        if (m_HitSignal != null)
        {
            m_HitSignal.SetActive(true);
        }
    }

    public void Downswing()
    {

        if (m_HitSignal != null)
        {
            m_HitSignal.SetActive(false);
        }

        m_Animator.SetTrigger("MeleeDownswingTrigger");
        m_DamageTrigger.m_CollisionHandler = this;
        m_DamageTrigger.m_Active = true;

        m_Timer = EndAfter(m_DownswingTime);
        StartCoroutine(m_Timer);
    }

    public void EndAttack()
    {
        m_DamageTrigger.m_Active = false;
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
        m_Callback.OnMeleeHitEnd();
        m_DamageTrigger.m_Active = false;
    }

    public void CancelHit()
    {
        if (m_Timer != null)
        {
            StopCoroutine(m_Timer);
            m_DamageTrigger.m_Active = false;
        }
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

                if (m_Timer != null)
                {
                    StopCoroutine(m_Timer);
                }
            }
        }
    }

    public void HandleCollision(Collider other, bool initialCollision)
    {
    }

    public interface MeleeHitCallback
    {
        void OnMeleeHitSuccess();
        void OnMeleeHitEnd();
    }


}

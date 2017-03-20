using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AngelMeleeAttack : AngelAttack, DamageCollisionHandler, Damage.DamageCallback {

    public Damage.BlockableType m_BlockableType = Damage.BlockableType.Parry;
    public Damage.DamageType m_DamageType = Damage.DamageType.Regular;

    public string m_AnimationName;

    public float m_DamageAmount;
    public BossMeleeDamage m_Damage;

    public float m_DownswingStartTime;
    public float m_ActivateDamageTimeAfterDownswing;
    public float m_TimeBeforeEnd;

    protected IEnumerator m_StateTimer;
    protected bool m_Cancelled;

    public bool m_TriggerIdleOnEnd = false;

    public bool m_PerfectTracking = false;

    public override void StartAttack()
    {
        base.StartAttack();

        m_Cancelled = false;

        m_Damage.m_Amount = AdjustDmg(this.m_DamageAmount);
        m_Damage.m_CollisionHandler = this;
        m_Damage.m_Callback = this;
        m_Damage.m_Active = false;
        m_Damage.m_Blockable = m_BlockableType;
        m_Damage.m_Type = m_DamageType;

        m_Animator.SetTrigger(m_AnimationName);

        m_StateTimer = StartDownswingAfterWaiting();
        StartCoroutine(m_StateTimer);
    }

    protected virtual IEnumerator StartDownswingAfterWaiting()
    {
        float t = 0;
        while((t += Time.deltaTime) < AdjustTime(m_DownswingStartTime))
        {
            if (m_PerfectTracking)
                m_FullTurnCommand.DoTurn();

            yield return null;
        }

        Downswing();
    }

    protected virtual void Downswing()
    {
        if (m_Cancelled)
            return;

        m_StateTimer = SetDamageActiveAfterWaiting();
        StartCoroutine(m_StateTimer);
    }

    protected virtual IEnumerator SetDamageActiveAfterWaiting()
    {
        float t = 0;
        while ((t += Time.deltaTime) < AdjustTime(m_ActivateDamageTimeAfterDownswing))
        {
            if (m_PerfectTracking)
                m_FullTurnCommand.DoTurn();
            yield return null;
        }

        SetDamageActive();
    }

    protected virtual void SetDamageActive()
    {
        if (m_Cancelled)
            return;

        m_Damage.m_Active = true;

        m_StateTimer = WaitBeforeAttackEnds();
        StartCoroutine(m_StateTimer);
    }

    protected virtual IEnumerator WaitBeforeAttackEnds()
    {
        yield return new WaitForSeconds(AdjustTime(m_TimeBeforeEnd));
        EndAttack();
    }

    protected virtual void EndAttack()
    {
        if (m_Cancelled)
            return;

        m_Damage.m_Active = false;
        m_Damage.m_Callback = null;
        m_Damage.m_CollisionHandler = null;
        
        if (m_TriggerIdleOnEnd)
            m_Animator.SetTrigger("IdleTrigger");

        m_Callback.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        m_Cancelled = true;

        if (m_StateTimer != null)
            StopCoroutine(m_StateTimer);

        m_Damage.m_Active = false;
        m_Damage.m_Callback = null;
        m_Damage.m_CollisionHandler = null;
    }

    public abstract void OnParryDamage();

    public abstract void OnBlockDamage();

    public virtual void OnSuccessfulHit()
    {
        m_SuccessLevel = 1;
        m_SuccessCallback.ReportResult(this);

        m_Damage.m_Active = false;

        CameraController.Instance.Shake();
    }

    public virtual void HandleScarletCollision(Collider other)
    {
        PlayerHittable hittable = other.GetComponent<PlayerHittable>();
        if (hittable != null && m_Damage.m_Active)
        {
            hittable.Hit(m_Damage);
        }
    }

    public abstract void HandleCollision(Collider other, bool initialCollision);

    public abstract void HandleScarletLeave(Collider other);
}

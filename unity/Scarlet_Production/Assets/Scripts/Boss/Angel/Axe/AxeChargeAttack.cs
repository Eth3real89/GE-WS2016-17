using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeChargeAttack : AngelAttack, DamageCollisionHandler, BossMeleeDamage.DamageCallback
{
    public enum State {None, Charge, Hit };

    public float m_StanceAndAimTime;

    public float m_ChargeTime;
    public TurnTowardsScarlet m_ChargeTurn;
    public float m_ChargeSpeed;

    public float m_UpswingTime;
    public float m_TimeAfterUpswing;

    public BossMeleeDamage m_Damage;
    public float m_DamageAmount;

    protected IEnumerator m_Timer;
    protected State m_State;

    protected bool m_ScarletInRange;

    public override void StartAttack()
    {
        base.StartAttack();
        m_State = State.None;
        m_ScarletInRange = false;

        m_Animator.SetTrigger("AxeChargeStanceTrigger");

        m_Timer = StanceAndAim();
        StartCoroutine(m_Timer);
    }

    protected IEnumerator StanceAndAim()
    {
        yield return new WaitForSeconds(AdjustTime(m_StanceAndAimTime));

        m_Animator.SetTrigger("AxeRunTrigger");

        m_Damage.m_Amount = this.m_DamageAmount;
        m_Damage.m_Active = true;
        m_Damage.m_Callback = this;
        m_Damage.m_CollisionHandler = this;

        m_State = State.Charge;
        m_Timer = Charge();
        StartCoroutine(m_Timer);
    }

    protected IEnumerator Charge()
    {
        float t = 0;
        while((t += Time.deltaTime) < AdjustTime(m_ChargeTime))
        {
            if (ScarletInRange())
                break;
            Move();

            yield return null;
        }

        m_Timer = ChargeStrike();
        StartCoroutine(m_Timer);
    }

    protected IEnumerator ChargeStrike()
    {
        m_Animator.SetTrigger("AxeChargeUpswingTrigger");

        yield return new WaitForSeconds(AdjustTime(m_UpswingTime));
        m_State = State.Hit;
        yield return new WaitForSeconds(AdjustTime(m_TimeAfterUpswing));

        m_Damage.m_Active = false;
        m_Damage.m_Callback = null;
        m_Damage.m_CollisionHandler = null;

        m_Timer = WaitAfterHit();
        StartCoroutine(m_Timer);
    }

    protected IEnumerator WaitAfterHit()
    {
        yield return new WaitForSeconds(AdjustTime(m_TimeAfterUpswing));

        if (m_SuccessLevel < 1)
        {
            m_SuccessLevel = -1;
        }

        m_Callback.OnAttackEnd(this);
    }

    public void OnSuccessfulHit()
    {
        m_SuccessLevel = 1;
        m_SuccessCallback.ReportResult(this);

        m_Damage.m_Active = false;
        m_Damage.m_Callback = null;
        m_Damage.m_CollisionHandler = null;

        // will most likely be cancelled immediately after this.
    }

    public void HandleScarletCollision(Collider other)
    {
        if (m_State == State.Charge)
        {
            m_ScarletInRange = true;
        }
        else if (m_State == State.Hit)
        {
            PlayerHittable hittable = other.GetComponent<PlayerHittable>();
            if (hittable != null)
            {
                hittable.Hit(m_Damage);
            }

            m_Damage.m_Active = false;
            m_Damage.m_Callback = null;
            m_Damage.m_CollisionHandler = null;
        }
    }

    protected void Move()
    {
        m_ChargeTurn.DoTurn();
        m_Boss.transform.position += m_Boss.transform.forward * AdjustSpeed(m_ChargeSpeed) * Time.deltaTime;
    }

    protected bool ScarletInRange()
    {
        return m_ScarletInRange;
    }

    public override void CancelAttack()
    {
        if (m_Timer != null)
            StopCoroutine(m_Timer);

        m_State = State.None;

        m_ScarletInRange = false;
        m_Damage.m_Active = false;
        m_Damage.m_CollisionHandler = null;
        m_Damage.m_Callback = null;

    }

    public void OnParryDamage()
    {
    }

    public void OnBlockDamage()
    {
    }

    public void HandleCollision(Collider other, bool initialCollision)
    {
    }

    public void HandleScarletLeave(Collider other)
    {
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeChargeFollowUpSuccess : AngelAttack, DamageCollisionHandler, Damage.DamageCallback
{
    
    public float m_FlyHeight;
    public float m_FlyForwardSpeed;
    public float m_FlyTime;
    public float m_TimeAfterLand;
    protected IEnumerator m_FlyTimer;

    public float m_ActivateDamageTime;
    protected IEnumerator m_DamageActiveTimer;
    public float m_TimeDamageActive;

    public BossMeleeDamage m_Damage;
    public float m_DamageAmount;

    protected float m_YPosBefore;

    public override void StartAttack()
    {
        base.StartAttack();

        m_DamageActiveTimer = ActivateDamage();
        StartCoroutine(m_DamageActiveTimer);

        m_FlyTimer = Fly();
        StartCoroutine(m_FlyTimer);
    }

    protected IEnumerator Fly()
    {
        m_YPosBefore = m_Boss.transform.position.y;

        m_Animator.SetTrigger("AxeChargeOverheadTrigger");

        float t = 0;
        while((t += Time.deltaTime) < AdjustTime(m_FlyTime))
        {
            float yPosBefore = m_YPosBefore + Mathf.Sin(m_FlyHeight * t / AdjustTime(m_FlyTime) * Mathf.PI);

            Vector3 newPos = m_Boss.transform.position + m_Boss.transform.forward * m_FlyForwardSpeed * Time.deltaTime;
            newPos.y = yPosBefore;

            m_Boss.transform.position = newPos;
            yield return null;
        }

        m_Boss.transform.position = new Vector3(m_Boss.transform.position.x, m_YPosBefore, m_Boss.transform.position.z);
        yield return new WaitForSeconds(m_TimeAfterLand);

        m_Callback.OnAttackEnd(this);
    }

    protected IEnumerator ActivateDamage()
    {
        yield return new WaitForSeconds(m_ActivateDamageTime);

        m_Damage.m_Amount = AdjustDmg(this.m_DamageAmount);
        m_Damage.m_Active = false;
        m_Damage.m_Callback = null;
        m_Damage.m_CollisionHandler = null;

        yield return new WaitForSeconds(m_TimeDamageActive);
    }

    public void HandleScarletCollision(Collider other)
    {
        PlayerHittable hittable = other.GetComponent<PlayerHittable>();
        if (hittable != null)
        {
            hittable.Hit(m_Damage);
        }
    }

    public void HandleCollision(Collider other, bool initialCollision)
    {
    }

    public void HandleScarletLeave(Collider other)
    {
    }

    public void OnParryDamage()
    {
    }

    public void OnBlockDamage()
    {
    }

    public void OnSuccessfulHit()
    {
        m_SuccessLevel = 1;
    }

    public override void CancelAttack()
    {
        m_Damage.m_Active = false;
        m_Damage.m_Callback = null;
        m_Damage.m_CollisionHandler = null;

        if (m_DamageActiveTimer != null)
            StopCoroutine(m_DamageActiveTimer);

        if (m_FlyTimer != null)
            StopCoroutine(m_FlyTimer);
    }
}

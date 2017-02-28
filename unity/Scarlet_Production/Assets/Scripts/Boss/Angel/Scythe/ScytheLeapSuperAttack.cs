using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheLeapSuperAttack : AngelAttack, BossMeleeDamage.DamageCallback, DamageCollisionHandler {

    public float m_TimeBeforeRotation;

    public float m_RotateAngles;
    public float m_RotationTime;
    public float m_UpSpeed;

    public float m_SwingTimeBeforeDamage;

    public float m_TimeDamageActive;

    public float m_TimeEnd;

    protected IEnumerator m_Timer;

    public BossMeleeDamage m_ScytheRangeTrigger;
    public AngelDamage m_RotationDamage;
    public AngelDamage m_DownswingDamage; 

    public override void StartAttack()
    {
        base.StartAttack();

        m_Animator.SetTrigger("ScytheSuperRotationTrigger");

        m_RotationDamage.m_Callback = this;
        m_DownswingDamage.m_Callback = this;

        m_Timer = WaitBeforeRotate();
        StartCoroutine(m_Timer);
    }

    protected virtual IEnumerator WaitBeforeRotate()
    {
        yield return new WaitForSeconds(m_TimeBeforeRotation);

        m_Timer = Rotate();
        StartCoroutine(m_Timer);
    }

    protected virtual IEnumerator Rotate()
    {
        float initialYRotation = m_Boss.transform.rotation.eulerAngles.y;
        Vector3 posBefore = m_Boss.transform.position + Vector3.zero;

        bool overheadStarted = false;
        
        m_ScytheRangeTrigger.m_CollisionHandler = this;
        m_ScytheRangeTrigger.m_Active = true;

        m_RotationDamage.m_Active = true;        

        Rigidbody b = m_Boss.GetComponent<Rigidbody>();

        float stopUpwardsTime = 0.5f;
        float t = 0;
        while((t += Time.deltaTime) < m_RotationTime)
        {
            m_Boss.transform.rotation = Quaternion.Euler(0, initialYRotation + m_RotateAngles * (t / m_RotationTime), 0);
            
            if (!overheadStarted && t > m_RotationTime - stopUpwardsTime)
            {
                m_Animator.SetTrigger("ScytheSuperOverheadTrigger");
                overheadStarted = true;
            }

            if (overheadStarted)
            {
                m_Boss.transform.position = Vector3.Lerp(m_Boss.transform.position, posBefore, (t - (m_RotationTime - 0.15f)) / stopUpwardsTime);
            }
            else
            {
                m_Boss.transform.position += Vector3.up * m_UpSpeed * Time.deltaTime;
            }

            yield return null;
        }

        m_Boss.transform.rotation = Quaternion.Euler(0, m_RotateAngles + initialYRotation, 0);
        m_Boss.transform.position = posBefore;

        m_RotationDamage.m_Active = false;

        if (!overheadStarted)
            m_Animator.SetTrigger("ScytheSuperOverheadTrigger");

        m_Timer = SwingScythe();
        StartCoroutine(m_Timer);
    }

    protected virtual IEnumerator SwingScythe()
    {
        yield return new WaitForSeconds(m_SwingTimeBeforeDamage);
        m_DownswingDamage.m_Active = true;

        yield return new WaitForSeconds(m_TimeDamageActive);
        m_DownswingDamage.m_Active = false;

        if (m_SuccessLevel < 1)
        {
            m_SuccessLevel = 0;
            m_SuccessCallback.ReportResult(this);
        }
        else
        {
            m_Timer = EndAttack();
            StartCoroutine(m_Timer);
        }
    }

    protected virtual IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(m_TimeEnd);

        m_Animator.SetTrigger("IdleTrigger");
        CleanUp();

        m_Callback.OnAttackEnd(this);
   }

    private void CleanUp()
    {
        m_ScytheRangeTrigger.m_CollisionHandler = null;
        m_ScytheRangeTrigger.m_Active = false;

        m_DownswingDamage.m_Callback = null;
        m_DownswingDamage.m_Active = false;

        m_RotationDamage.m_Callback = null;
        m_RotationDamage.m_Active = false;
    }

    public override void CancelAttack()
    {
        StopAllCoroutines();

        CleanUp();

        if (m_Timer != null)
            StopCoroutine(m_Timer);
    }

    public void OnParryDamage()
    {
    }

    public void OnBlockDamage()
    {
    }

    public void OnSuccessfulHit()
    {
        if (m_DownswingDamage.m_Active)
        {
            m_SuccessLevel = 1;
            m_SuccessCallback.ReportResult(this);

            m_DownswingDamage.m_Active = false;
        }
    }

    public void HandleScarletCollision(Collider other)
    {
        if (m_RotationDamage.m_Active)
        {
            PlayerHittable hittable = other.GetComponentInChildren<PlayerHittable>();
            if (hittable != null)
            {
                hittable.Hit(m_RotationDamage);
                m_RotationDamage.m_Active = false;
            }
        }

        if (m_DownswingDamage.m_Active)
        {
            PlayerHittable hittable = other.GetComponentInChildren<PlayerHittable>();
            if (hittable != null)
            {
                hittable.Hit(m_DownswingDamage);
                m_DownswingDamage.m_Active = false;
            }
        }
    }

    public void HandleCollision(Collider other, bool initialCollision)
    {
    }

    public void HandleScarletLeave(Collider other)
    {
    }
}

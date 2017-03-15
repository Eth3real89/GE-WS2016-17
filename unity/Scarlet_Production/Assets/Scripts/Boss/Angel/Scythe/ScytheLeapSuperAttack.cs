using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheLeapSuperAttack : AngelAttack, BossMeleeDamage.DamageCallback, DamageCollisionHandler {

    public float m_TimeBeforeRotation;

    public float m_RotateAngles;
    public float m_RotationTime;
    public float m_UpSpeed;
    protected float m_ForwardSpeed;

    public float m_TimeStuckInAir;

    public float m_DownwardsTime;

    public float m_TimeDamageActive;
    public float m_TimeEnd;

    protected bool m_VisualEffectDisabled;

    protected IEnumerator m_Timer;

    public BossMeleeDamage m_ScytheRangeTrigger;
    public GameObject m_Scarlet;
    public AngelDamage m_RotationDamage;
    public AngelDamage m_DownswingDamage;

    public float m_DamageAmountRotation = 30f;
    public float m_DamageAmountDownswing = 90f;

    protected float m_YPosBefore;

    public override void StartAttack()
    {
        base.StartAttack();

        m_Animator.SetTrigger("ScytheSuperRotationTrigger");

        m_RotationDamage.m_Callback = this;
        m_DownswingDamage.m_Callback = this;

        m_RotationDamage.m_DamageAmount = AdjustDmg(this.m_DamageAmountRotation);
        m_DownswingDamage.m_DamageAmount = AdjustDmg(this.m_DamageAmountDownswing);

        m_VisualEffectDisabled = false;

        m_Timer = WaitBeforeRotate();
        StartCoroutine(m_Timer);
    }

    protected virtual IEnumerator WaitBeforeRotate()
    {
        yield return new WaitForSeconds(AdjustTime(m_TimeBeforeRotation));

        m_Timer = Rotate();
        StartCoroutine(m_Timer);
    }

    protected virtual IEnumerator Rotate()
    {
        float initialYRotation = m_Boss.transform.rotation.eulerAngles.y;
        m_YPosBefore = m_Boss.transform.position.y;

        m_ForwardSpeed = Vector3.Distance(m_Boss.transform.position, m_Scarlet.transform.position) / AdjustTime((m_RotationTime + m_DownwardsTime));
        
        m_ScytheRangeTrigger.m_CollisionHandler = this;
        m_ScytheRangeTrigger.m_Active = true;

        m_RotationDamage.m_Active = true;        

        Rigidbody b = m_Boss.GetComponent<Rigidbody>();
        b.useGravity = false;

        float yVelocity = 0;
        float rotationTime = AdjustTime(m_RotationTime);

        float t = 0;
        while((t += Time.deltaTime) < rotationTime)
        {
            yVelocity += ((t > rotationTime / 2)? -1 : 1) * Time.deltaTime * m_UpSpeed;
            m_Boss.transform.position += new Vector3(0, yVelocity, 0) + m_Boss.transform.forward * AdjustSpeed(m_ForwardSpeed) * Time.deltaTime;

            m_Boss.transform.rotation = Quaternion.Euler(0, initialYRotation + m_RotateAngles * Mathf.Sin(t / rotationTime * Mathf.PI / 2), 0);
            yield return null;
        }

        m_Boss.transform.rotation = Quaternion.Euler(0, m_RotateAngles + initialYRotation, 0);

        m_RotationDamage.m_Active = false;

        m_Timer = AtHighestPoint();
        StartCoroutine(m_Timer);
    }

    protected virtual IEnumerator AtHighestPoint()
    {
        m_Animator.SetFloat("AnimationSpeed", AdjustSpeed(1 / AdjustTime((m_TimeStuckInAir + m_DownwardsTime))));
        m_Animator.SetTrigger("ScytheSuperOverheadTrigger");

        float t = 0;
        while((t += Time.deltaTime) < AdjustTime(m_TimeStuckInAir))
        {
            float angle = BossTurnCommand.CalculateAngleTowards(m_Boss.transform, m_Scarlet.transform);
            m_Boss.transform.Rotate(Vector3.up, angle);
            
            yield return null;
        }

        m_Timer = ComeCrashingDown();
        StartCoroutine(m_Timer);
    }

    protected virtual IEnumerator ComeCrashingDown()
    {
        m_DownswingDamage.m_Active = true;

        Vector3 xzAfter = new Vector3(m_Scarlet.transform.position.x, 0, m_Scarlet.transform.position.z);

        Vector3 posBefore = m_Boss.transform.position + Vector3.zero;

        float downwardsTime = AdjustTime(m_DownwardsTime);

        float t = 0;
        while ((t += Time.deltaTime) < downwardsTime)
        {
            Vector3 newPos = Vector3.Lerp(posBefore, xzAfter, t / downwardsTime);
            newPos.y = Mathf.Lerp(posBefore.y, m_YPosBefore, Mathf.Sin(t / downwardsTime * Mathf.PI / 2));

            m_Boss.transform.position = newPos;

            yield return null;
        }

        m_Animator.SetFloat("AnimationSpeed", AdjustSpeed(1f));
        DisableVisualEffect();
        m_Boss.transform.position = m_Boss.transform.position - new Vector3(0, m_Boss.transform.position.y + m_YPosBefore, 0);

        Rigidbody b = m_Boss.GetComponent<Rigidbody>();
        b.useGravity = true;

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

    private void DisableVisualEffect()
    {
        if (!m_VisualEffectDisabled)
        {
            m_Boss.GetComponent<ControlAngelVisualisation>().DisableSpecialVisualisation();
            m_VisualEffectDisabled = true;
        }
    }

    protected virtual IEnumerator EndAttack()
    {
        Rigidbody b = m_Boss.GetComponent<Rigidbody>();
        b.useGravity = false;

        yield return new WaitForSeconds(AdjustTime(m_TimeEnd));

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

        Rigidbody b = m_Boss.GetComponent<Rigidbody>();
        b.useGravity = true;
        m_Animator.SetFloat("AnimationSpeed", 1f);
    }

    public override void CancelAttack()
    {
        StopAllCoroutines();

        CleanUp();
        DisableVisualEffect();

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

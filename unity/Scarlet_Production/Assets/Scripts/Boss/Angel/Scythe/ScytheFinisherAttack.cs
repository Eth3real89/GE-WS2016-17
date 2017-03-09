using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheFinisherAttack : AngelAttack, Damage.DamageCallback, DamageCollisionHandler {

    public BossMeleeDamage m_Damage;
    public Damage.DamageType m_DamageType = Damage.DamageType.Special;
    public Damage.BlockableType m_BlockableType = Damage.BlockableType.OnlyBlock;
    public float m_DamageAmount;

    public float m_TimeBeforeDamageActive;
    public float m_FirstRotationTime;
    public float m_TimeAfterFirstRotation;

    public float m_SecondRotationTime;
    public float m_TimeAfterSecondRotation;

    public float m_ThirdRotationTime;
    public float m_WindupTime;

    protected IEnumerator m_StateTimer;
    protected IEnumerator m_RotationTimer;

    public override void StartAttack()
    {
        base.StartAttack();
        m_Animator.SetTrigger("ScytheFinisherAttackTrigger");

        m_StateTimer = FirstRotation();
        StartCoroutine(m_StateTimer);
    }

    protected IEnumerator FirstRotation()
    {
        yield return new WaitForSeconds(m_TimeBeforeDamageActive / 2);
        yield return StartCoroutine(Rotation(m_TimeBeforeDamageActive / 2, -1, 40));
        
        ActivateDamage();

        m_RotationTimer = Rotation(m_FirstRotationTime, 1, -400);
        yield return StartCoroutine(m_RotationTimer);

        if (m_TimeAfterFirstRotation > 0)
            yield return new WaitForSeconds(m_TimeAfterFirstRotation);

        m_StateTimer = SecondRotation();
        StartCoroutine(m_StateTimer);
    }

    protected IEnumerator SecondRotation()
    {
        m_RotationTimer = Rotation(m_SecondRotationTime, 0);
        yield return StartCoroutine(m_RotationTimer);

        if (m_TimeAfterSecondRotation > 0)
            yield return new WaitForSeconds(m_TimeAfterSecondRotation);

        m_StateTimer = ThirdRotation();
        StartCoroutine(m_StateTimer);
    }

    protected IEnumerator ThirdRotation()
    {
        m_RotationTimer = Rotation(m_ThirdRotationTime, -1);
        yield return StartCoroutine(m_RotationTimer);

        ResetDamage();
        m_Animator.SetTrigger("ScytheFinisherWindupTrigger");
        yield return new WaitForSeconds(m_WindupTime);
        EndAttack();
    }

    protected IEnumerator Rotation(float time, int acceleration, float angle = -360)
    {
        float rotationBefore = m_Boss.transform.rotation.eulerAngles.y;

        float t = 0;
        while((t += Time.deltaTime) < time)
        {
            float newAngle;
            if (acceleration == 0)
                newAngle = rotationBefore + (t / time) * angle;
            else if (acceleration == -1)
                newAngle = rotationBefore + Mathf.Pow(t / time, 1 / 3f) * angle;
            else
                newAngle = rotationBefore + Mathf.Pow(t / time, 3f) * angle;

            m_Boss.transform.rotation = Quaternion.Euler(0, newAngle, 0);

            yield return null;
        }

        m_Boss.transform.rotation = Quaternion.Euler(0, rotationBefore + angle, 0);
        yield return null;
    }

    protected void EndAttack()
    {
        m_Callback.OnAttackEnd(this);
    }

    protected void ActivateDamage()
    {
        m_Damage.m_Callback = this;
        m_Damage.m_CollisionHandler = this;

        m_Damage.m_BlockType = this.m_BlockableType;
        m_Damage.m_Type = this.m_DamageType;

        m_Damage.m_Amount = this.m_DamageAmount;

        m_Damage.m_Active = true;
    }

    public override void CancelAttack()
    {
        ResetDamage();

        if (m_StateTimer != null)
            StopCoroutine(m_StateTimer);

        if (m_RotationTimer != null)
            StopCoroutine(m_RotationTimer);
    }

    protected void ResetDamage()
    {
        m_Damage.m_Active = false;
        m_Damage.m_Callback = null;
        m_Damage.m_CollisionHandler = null;
    }

    public void OnParryDamage()
    {
    }

    public void OnBlockDamage()
    {
    }

    public void OnSuccessfulHit()
    {
        // stagger or sth.
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
}

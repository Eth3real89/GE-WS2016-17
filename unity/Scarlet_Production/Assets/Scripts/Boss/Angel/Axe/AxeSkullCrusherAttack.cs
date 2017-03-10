using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeSkullCrusherAttack : AngelMeleeAttack
{

    public float m_HitGroundTime;
    protected IEnumerator m_HitGroundEnumerator;

    protected override void SetDamageActive()
    {
        if (m_Cancelled)
            return;

        m_Damage.m_Active = true;

        m_StateTimer = HitGround();
        StartCoroutine(m_StateTimer);
    }

    protected IEnumerator HitGround()
    {
        yield return new WaitForSeconds(m_HitGroundTime);
        m_Damage.m_Active = false;
        m_Damage.m_CollisionHandler = null;
        m_Damage.m_Callback = null;

        if (m_SuccessLevel < 1)
        {
            m_SuccessLevel = -1;
            m_SuccessCallback.ReportResult(this);
        }

        m_StateTimer = WaitBeforeAttackEnds();
        StartCoroutine(m_StateTimer);
    }

    public override void HandleScarletCollision(Collider other)
    {
        base.HandleScarletCollision(other);
    }

    public override void OnSuccessfulHit()
    {
        base.OnSuccessfulHit();
    }

    public override void HandleCollision(Collider other, bool initialCollision)
    {
    }

    public override void HandleScarletLeave(Collider other)
    {
    }

    public override void OnBlockDamage()
    {
    }

    public override void OnParryDamage()
    {
    }

}

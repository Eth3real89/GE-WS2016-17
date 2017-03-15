using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerOverheadAttack : AngelMeleeAttack {

    public float m_TimeStartFlyingInDownswing;
    public float m_HitGroundTime;
    protected IEnumerator m_HitGroundEnumerator;

    public float m_YToReach;
    public float m_ForwardSpeed;

    protected float m_YBefore;

    protected override IEnumerator StartDownswingAfterWaiting()
    {
        m_YBefore = m_Boss.transform.position.y;

        float flyTime = AdjustTime(m_DownswingStartTime - m_TimeStartFlyingInDownswing);

        float t = 0;
        while ((t += Time.deltaTime) < AdjustTime(m_TimeStartFlyingInDownswing))
        {
            if (m_PerfectTracking)
                m_FullTurnCommand.DoTurn();
            yield return null;
        }

        t = 0;
        while((t += Time.deltaTime) < flyTime)
        {
            Vector3 newPos = m_Boss.transform.position + m_Boss.transform.forward * flyTime * Time.deltaTime;
            newPos.y = m_YBefore + Mathf.Sin(t / flyTime * Mathf.PI / 4f) * m_YToReach;

            if (m_PerfectTracking)
                m_FullTurnCommand.DoTurn();

            m_Boss.transform.position = newPos;
            yield return null;
        }

        Downswing();
    }

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
        float t = 0;
        while ((t += Time.deltaTime) < AdjustTime(m_HitGroundTime))
        {
            Vector3 newPos = m_Boss.transform.position + m_Boss.transform.forward * AdjustSpeed(m_ForwardSpeed) * Time.deltaTime;
            newPos.y = m_YBefore + Mathf.Sin(Mathf.PI / 4 + t / AdjustTime(m_HitGroundTime) * Mathf.PI / 4f) * m_YToReach;

            if (m_PerfectTracking)
                m_FullTurnCommand.DoTurn();

            m_Boss.transform.position = newPos;
            yield return null;
        }

        m_Boss.transform.position -= new Vector3(0, m_Boss.transform.position.y - m_YBefore, 0);

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
        m_Damage.m_Active = false;
    }

    public override void OnParryDamage()
    {
        m_Damage.m_Active = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelOppositeMovementAttack : AngelAttack
{

    public Transform m_ArenaCenter;
    public Transform m_Scarlet;
    public float m_MaxSpeed;

    public float m_DistanceFromCenter;

    public float m_Time;
    protected IEnumerator m_Timer;

    public override void StartAttack()
    {
        base.StartAttack();

        m_Timer = MoveOppositeScarlet();
        StartCoroutine(m_Timer);
    }

    protected IEnumerator MoveOppositeScarlet()
    {
        float t = 0;
        while((t += Time.deltaTime) < AdjustTime(m_Time))
        {
            Vector3 idealPos = m_ArenaCenter.position + ((m_ArenaCenter.position - m_Scarlet.position)).normalized * m_DistanceFromCenter;
            idealPos.y = m_Boss.transform.position.y;

            m_FullTurnCommand.DoTurn();

            Vector3 distance = idealPos - m_Boss.transform.position;
            if (distance.magnitude < Time.deltaTime * AdjustSpeed(m_MaxSpeed))
            {
                m_Boss.transform.position = idealPos;
            }
            else
            {
                m_Boss.transform.position += distance.normalized * Time.deltaTime * AdjustSpeed(m_MaxSpeed);
            }

            if (t >= 0.1 * AdjustSpeed(m_Time) && Vector3.Distance(m_Scarlet.position, m_Boss.transform.position) < 2)
            {
                StopUnsuccessfully();
            }
            
            yield return null;
        }

        m_SuccessLevel = 1;
        m_SuccessCallback.ReportResult(this);
        m_Callback.OnAttackEnd(this);
    }

    private void StopUnsuccessfully()
    {
        if (m_Timer != null)
            StopCoroutine(m_Timer);

        m_SuccessLevel = -1;
        m_SuccessCallback.ReportResult(this);
        m_Callback.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        if (m_Timer != null)
            StopCoroutine(m_Timer);
    }
}

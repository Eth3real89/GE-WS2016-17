using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelOppositeMovementAttack : AngelAttack
{

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
        while((t += Time.deltaTime) < m_Time)
        {

            yield return null;
        }

        m_Callback.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        if (m_Timer != null)
            StopCoroutine(m_Timer);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelDashAwayAttack : AngelAttack {

    public float m_MaxSpeed;
    public Transform[] m_Targets;

    protected IEnumerator m_Enumerator;

    public override void StartAttack()
    {
        base.StartAttack();
        m_Enumerator = DoDash();
        StartCoroutine(m_Enumerator);
    }

    protected virtual IEnumerator DoDash()
    {
        Vector3 desiredPosition = m_Targets[UnityEngine.Random.Range(0, m_Targets.Length)].position;
        desiredPosition.y = m_Boss.transform.position.y;

        while (true)
        {
            m_FullTurnCommand.DoTurn();

            if (CloseEnough(desiredPosition))
            {
                break;
            }

            m_Boss.transform.position += (desiredPosition - m_Boss.transform.position).normalized * m_MaxSpeed * Time.deltaTime;

            yield return null;
        }

        m_Callback.OnAttackEnd(this);
    }

    private bool CloseEnough(Vector3 desiredPosition)
    {
        return Vector3.Distance(m_Boss.transform.position, desiredPosition) <= m_MaxSpeed * Time.deltaTime * 1.5;
    }

    public override void CancelAttack()
    {

    }
}

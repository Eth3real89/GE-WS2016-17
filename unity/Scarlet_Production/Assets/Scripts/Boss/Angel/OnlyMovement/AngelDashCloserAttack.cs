using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelDashCloserAttack : AngelAttack {

    public float m_MaxSpeed;
    public Transform m_Scarlet;
    public bool m_LeftOfScarlet;

    protected IEnumerator m_Enumerator;

    public override void StartAttack()
    {
        base.StartAttack();
        m_Enumerator = DoDash();
        StartCoroutine(m_Enumerator);
    }

    protected virtual IEnumerator DoDash()
    {
        while(true)
        {
            Vector3 desiredPosition = m_Scarlet.transform.position + new Vector3(1, 0, 0) * 0.5f * (m_LeftOfScarlet ? -1 : 1);
            desiredPosition.y = m_Boss.transform.position.y;

            m_FullTurnCommand.DoTurn();

            if(CloseEnough(desiredPosition))
            {
                break;
            }

            m_Boss.transform.position += (desiredPosition - m_Boss.transform.position).normalized * AdjustSpeed(m_MaxSpeed) * Time.deltaTime;

            yield return null;
        }

        m_Callback.OnAttackEnd(this);
    }

    private bool CloseEnough(Vector3 desiredPosition)
    {
        return Vector3.Distance(m_Boss.transform.position, desiredPosition) <= AdjustSpeed(m_MaxSpeed) * Time.deltaTime * 1.5;
    }

    public override void CancelAttack()
    {
        if (m_Enumerator != null)
            StopCoroutine(m_Enumerator);
    }

}

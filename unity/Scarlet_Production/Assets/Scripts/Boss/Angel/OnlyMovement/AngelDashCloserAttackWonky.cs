using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelDashCloserAttackWonky : AngelDashCloserAttack
{
    
    public float m_WonkinessEffect = 0f;

    protected float m_StartDistance;
    
    public override void StartAttack()
    {
        m_StartDistance = Vector3.Distance(m_Scarlet.transform.position + new Vector3(1, 0, 0), m_Boss.transform.position);

        m_Enumerator = DoDash();
        StartCoroutine(m_Enumerator);
    }

    protected override Vector3 CalculateMovement(Vector3 desiredPosition)
    {
        Vector3 movement = (desiredPosition - m_Boss.transform.position).normalized;
        Vector3 wonkyMovement = m_Boss.transform.right * Mathf.Sin(Vector3.Distance(desiredPosition, m_Boss.transform.position) / m_StartDistance * Mathf.PI / 2) * (m_LeftOfScarlet ? 1 : -1);
        movement = movement + m_WonkinessEffect * wonkyMovement;

        return movement;
    }

}

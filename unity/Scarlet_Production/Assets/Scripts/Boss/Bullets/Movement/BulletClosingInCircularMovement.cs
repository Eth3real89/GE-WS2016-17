using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletClosingInCircularMovement : BulletCircularMovement {

    public float m_CloseInSpeed = 2f;
    public float m_CloseInToRadius = 1f;

    protected float m_Time = -1;

    public override void HandleMovement(BulletBehaviour b)
    {
        if (m_Time == -1)
        {
            m_Time = 0;
        }
        else
        {
            m_Time += Time.deltaTime;
        }

        if (m_Time < m_TimeToGetInFormation)
        {
            base.HandleMovement(b);
        }
        else if (m_Time - m_TimeToGetInFormation < m_CloseInSpeed)
        {
            float timeSinceClosingIn = m_Time - m_TimeToGetInFormation;
            float radiusShouldBe = Mathf.Lerp(m_Radius, m_CloseInToRadius, timeSinceClosingIn / m_CloseInSpeed);
            
            float currentAngles = 360 * (1f - m_IndexInCircle / (float)(m_TotalBulletsInCircle)) + 360 * timeSinceClosingIn / m_CloseInSpeed;

            Vector3 move;
            Vector3 help = Quaternion.Euler(0, currentAngles, 0) * new Vector3(0, 0, radiusShouldBe);
            if (m_LastMovement.y == VAL_LAST_UNINITIALIZED)
            {
                move = -m_InitialPos + m_Center + help;
            }
            else
            {
                move = -m_LastMovement + help;
            }

            m_LastMovement = help;
            Vector3 adjustedOffsetChange = m_CenterOffset * (m_PassedTime / m_TimeToGetInFormation) - m_OffsetCleared;
            m_OffsetCleared += adjustedOffsetChange;

            b.MoveBy(move + adjustedOffsetChange);
        }
    }

}

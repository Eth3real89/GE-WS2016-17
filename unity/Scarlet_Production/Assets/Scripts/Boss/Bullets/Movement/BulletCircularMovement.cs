using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCircularMovement : BulletMovement {

    // because vector3 cant be null & this value will never occur
    private const int VAL_LAST_UNINITIALIZED = 100000;

    public float m_Radius;
    public float m_TimeToGetInFormation;

    public Vector3 m_Center;
    private Vector3 m_CenterOffset;
    private Vector3 m_OffsetCleared;

    private float m_AnglesToReach;
    private float m_InitialAngle;

    public int m_IndexInCircle;
    public int m_TotalBulletsInCircle;

    private bool m_FirstMovement = true;
    private float m_PassedTime;

    private Vector3 m_InitialPos;
    private Vector3 m_LastMovement;

    private Vector3 m_UnrelatedMovement;
    private Vector3 m_LastPosition;

    public Vector3 CalculateCenter(BulletBehaviour b)
    {
        m_Center = b.transform.position + b.transform.forward.normalized * m_Radius;
        return m_Center;
    }

    public Vector3 CalculateCenterOffset()
    {
        Vector3 offset = new Vector3(m_UnrelatedMovement.x, 0, m_UnrelatedMovement.z);
        print(offset);
        return offset;
    }

    public void SetCenterOffset(Vector3 offset)
    {
        m_CenterOffset = offset;
        m_OffsetCleared = new Vector3(0, 0, 0);
    }

    public override void HandleMovement(BulletBehaviour b)
    {
        if (m_FirstMovement)
        {
            m_InitialPos = new Vector3(b.transform.position.x, b.transform.position.y, b.transform.position.z);

            m_InitialAngle = BossTurnCommand.CalculateAngleTowards(b.transform.position, m_Center) + 180;
            m_PassedTime = 0;

            m_AnglesToReach = 360f * (1f - m_IndexInCircle / (float) (m_TotalBulletsInCircle));
            m_FirstMovement = false;
            m_LastMovement = new Vector3(0, VAL_LAST_UNINITIALIZED, 0);
            m_UnrelatedMovement = new Vector3(0, 0, 0);
        }
        else
        {
            m_UnrelatedMovement += (b.transform.position - m_LastPosition);

            m_PassedTime += Time.deltaTime;
            if (m_PassedTime > m_TimeToGetInFormation)
            {
                return;
            }

            float currentAngles = m_AnglesToReach * (m_PassedTime / m_TimeToGetInFormation) + m_InitialAngle;

            Vector3 move;
            Vector3 help = Quaternion.Euler(0, currentAngles, 0) * new Vector3(0, 0, m_Radius);
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
        m_LastPosition = b.transform.position;
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBeamishMovement : BulletMovement
{
    public Transform m_ToRotate;

    public int m_NumSways;
    public bool m_InitiallyAimAtScarlet;

    public float m_Angle;
    public float m_BuildupTime;
    public float m_RotationTime;
    private bool m_Started = false;

    private float m_CurrentAngleDelta;
    private float m_TimeSinceStart;

    private float m_PrevAngleChange = 0f; 

    public override void HandleMovement(BulletBehaviour b)
    {
        if (!m_Started)
        {
            m_Started = true;

            m_TimeSinceStart = 0;

            if (m_InitiallyAimAtScarlet)
            {
                m_ToRotate.Rotate(Vector3.up, 0f);
            }
            else
            {
                m_ToRotate.Rotate(Vector3.up, -m_Angle / 2f);
            }
        }
        else
        {
            m_TimeSinceStart += Time.deltaTime;
        }

        if (m_TimeSinceStart < m_BuildupTime)
            return;

        if (m_InitiallyAimAtScarlet)
        {
            m_CurrentAngleDelta = Mathf.Sin(((m_TimeSinceStart - m_BuildupTime) / m_RotationTime * m_NumSways / 2f) * 360 * Mathf.Deg2Rad) * m_Angle / 2;
        }
        else
        {
            m_CurrentAngleDelta = Mathf.Cos(((m_TimeSinceStart - m_BuildupTime) / m_RotationTime * m_NumSways / 2f) * 360 * Mathf.Deg2Rad) * m_Angle / 2 - m_Angle / 2;
        }

        m_ToRotate.Rotate(Vector3.up, -m_CurrentAngleDelta + m_PrevAngleChange);
        m_PrevAngleChange = m_CurrentAngleDelta;
    }
}

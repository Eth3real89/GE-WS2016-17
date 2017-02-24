using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAngularMovement : BulletMovement {

    public float m_RotationTime;

    protected float m_TimeSinceStart = -1;
    protected float m_InitialRotation;

    public float m_Angles;

    public GameObject m_ToRotate;

    public override void HandleMovement(BulletBehaviour b)
    {
        if (m_TimeSinceStart == -1)
        {
            m_TimeSinceStart = 0;
            m_InitialRotation = m_ToRotate.transform.rotation.eulerAngles.y;
        }
        else if (m_TimeSinceStart < m_RotationTime)
        {
            m_TimeSinceStart += Time.deltaTime;

            if (m_TimeSinceStart < m_RotationTime)
            {
                m_ToRotate.transform.rotation = Quaternion.Euler(0, m_InitialRotation + m_Angles * (m_TimeSinceStart / m_RotationTime), 0);
            }
            else
            {
                m_ToRotate.transform.rotation = Quaternion.Euler(0, m_InitialRotation + m_Angles, 0);
            }
        }
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMultiStageMovement : BulletMovement {

    public BulletMovement[] m_Movements;
    public float[] m_Times;

    private float m_TimeSinceStart;
    private int m_CurrentIndex = -1;

    public override void HandleMovement(BulletBehaviour b)
    {
        if (m_CurrentIndex == -1)
        {
            for(int i = 0; i < m_Movements.Length; i++)
            {
                m_Movements[i] = Instantiate(m_Movements[i]);
            }

            m_TimeSinceStart = 0;
            m_CurrentIndex = 0;
        }
        else
        {
            m_TimeSinceStart += Time.deltaTime;

            if (m_CurrentIndex < m_Times.Length)
            {
                if (m_TimeSinceStart > m_Times[m_CurrentIndex])
                {
                    m_TimeSinceStart = 0;
                    m_CurrentIndex++;
                }
            }
        }

        m_Movements[m_CurrentIndex].HandleMovement(b);
    }

}

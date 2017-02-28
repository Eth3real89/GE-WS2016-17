using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGrenadeMovement : BulletMovement
{
    // general comment:
    /*
     * Could have used something like the stuff in boss jump command for more accurate movement;
     * instead, this kind of simulates the throwing curve via a sinus function
     * (which is far less accurate, but it does the trick & that allows setting the time way better!)
     */

    public float m_TimeToReachGoal;
    public Transform m_Goal;

    protected bool m_Started = false;

    protected float m_Time;

    // only "y"-plane distance is measured

    protected float m_TotalDistance;

    protected Vector3 m_InitialPos;
    protected Vector3 m_XZMovement;
    protected float m_YDistanceTotal;
    protected float m_InitialY;

    protected bool m_JustDrop = false;

    public override void HandleMovement(BulletBehaviour b)
    {
        if (m_JustDrop)
        {
            m_Time += Time.deltaTime;
            b.transform.position = Vector3.Lerp(b.transform.position, m_Goal.transform.position, Mathf.Min(m_Time, 1f));
        }
        else if (!m_Started)
        {
            m_Started = true;
            m_TotalDistance = Vector3.Distance(b.transform.position - new Vector3(0, b.transform.position.y, 0), m_Goal.transform.position - new Vector3(0, m_Goal.transform.position.y, 0));

            if (m_TotalDistance < 2)
            {
                m_JustDrop = true;
            }

            m_Time = 0;

            m_InitialPos = b.transform.position + new Vector3();
            m_InitialY = b.transform.position.y;

            m_XZMovement = m_Goal.transform.position - b.transform.position;
            m_YDistanceTotal = m_XZMovement.y;

            m_XZMovement.y = 0;
        }
        else if (m_Time < m_TimeToReachGoal)
        {
            float dt = Time.deltaTime;
            m_Time += dt;

            m_Time = Mathf.Min(m_Time, m_TimeToReachGoal);

            float xzDist = m_TotalDistance * (m_Time / m_TimeToReachGoal);

            Vector3 newPos = m_InitialPos + m_XZMovement.normalized * xzDist;
            newPos.y = m_InitialY + (m_YDistanceTotal * m_Time / m_TimeToReachGoal) + Mathf.Sin(m_Time / m_TimeToReachGoal * Mathf.PI) * 3f;

            if (m_YDistanceTotal < 0 && newPos.y < m_Goal.transform.position.y)
            {
                newPos.y = m_Goal.transform.position.y;
            }

            b.transform.position = newPos;
        }
        else
        {
            m_Time += Time.deltaTime;
            b.transform.position = Vector3.Lerp(b.transform.position, m_Goal.transform.position, Mathf.Min(m_Time - m_TimeToReachGoal, 1f));
        }
    }


}

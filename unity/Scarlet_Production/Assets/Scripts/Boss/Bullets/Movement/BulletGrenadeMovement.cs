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
    protected float m_Distance;

    protected Vector3 m_XZMovement;
    protected float m_YDistancePerSecond;
    protected float m_InitialY;

    public override void HandleMovement(BulletBehaviour b)
    {
        if (!m_Started)
        {
            m_Started = true;
            m_TotalDistance = Vector3.Distance(b.transform.position - new Vector3(0, b.transform.position.y, 0), m_Goal.transform.position - new Vector3(0, m_Goal.transform.position.y, 0));

            m_Distance = 0;
            m_Time = 0;

            m_InitialY = b.transform.position.y;

            m_XZMovement = m_Goal.transform.position - b.transform.position;
            m_YDistancePerSecond = m_XZMovement.y / m_TimeToReachGoal * 50;

            m_XZMovement.y = 0;
        }
        else if (m_Time < m_TimeToReachGoal)
        {
            m_Time += Time.deltaTime;

            float xzDist = m_TotalDistance * (Time.deltaTime / m_TimeToReachGoal);
            m_Distance += xzDist;

            Vector3 newPos = b.transform.position + m_XZMovement.normalized * xzDist;
            newPos.y = m_InitialY + (m_YDistancePerSecond * Time.deltaTime) + Mathf.Sin(m_Time / m_TimeToReachGoal * Mathf.PI) * 2f;

            b.transform.position = newPos;
        }
        else
        {
            m_Time += Time.deltaTime;
            b.transform.position = Vector3.Lerp(b.transform.position, m_Goal.transform.position, Mathf.Min(m_Time - m_TimeToReachGoal, 1f));
        }
    }


}

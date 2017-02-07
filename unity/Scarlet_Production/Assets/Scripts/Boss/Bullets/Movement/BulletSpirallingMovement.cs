using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpirallingMovement : BulletMovement
{

    public float m_Speed;
    public float m_TurnPerSecondStart;

    private float m_TurnPerSecond = -7; // -7 instead of -1 for the "undefined" value, because, you know... unpredictability is key!

    public float m_TurnPerSecondDiminishment;

    public override void HandleMovement(BulletBehaviour b)
    {
        if (m_TurnPerSecond == -7)
        {
            m_TurnPerSecond = m_TurnPerSecondStart;
        }
        else
        {
            m_TurnPerSecond -= m_TurnPerSecondDiminishment * Time.deltaTime;
        }
        b.transform.Rotate(Vector3.up, m_TurnPerSecond * Time.deltaTime);
        b.transform.position += transform.forward * (m_Speed * Time.deltaTime);
    }
}

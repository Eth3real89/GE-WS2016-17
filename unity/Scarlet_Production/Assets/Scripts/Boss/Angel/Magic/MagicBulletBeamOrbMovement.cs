using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBulletBeamOrbMovement : BulletMovement {

    public float m_MinMovementTime;
    public float m_MinYToExpire = 1f;

    protected float m_MovementTime = -1;

    public override void HandleMovement(BulletBehaviour b)
    {
        if (m_MovementTime < 0)
        {
            m_MovementTime = 0;
        }
        else
        {
            m_MovementTime += Time.deltaTime;
        }

        if (m_MovementTime >= m_MinMovementTime && b.transform.position.y <= m_MinYToExpire)
        {
            b.Kill();
        }
    }
}

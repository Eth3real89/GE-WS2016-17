using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHomingMovement : BulletMovement {

    public Transform m_Target;
    
    public float m_HomingFactor;
    public float m_Speed;

    public override void HandleMovement(BulletBehaviour b)
    {
        Vector3 difference = m_Target.position - b.transform.position;

        float angle = BossTurnCommand.CalculateAngleTowards(b.transform, m_Target);

        if (angle < -180) angle += 360;
        else if (angle > 180) angle -= 360;

        float maxRotation = m_HomingFactor * Time.deltaTime;

        if (maxRotation > Mathf.Abs(angle))
        {
            b.transform.Rotate(Vector3.up, angle);
        }
        else
        {
            b.transform.Rotate(Vector3.up, (angle < 0? -1 : 1) * maxRotation);
        }

        Vector3 move = b.transform.forward * m_Speed * Time.deltaTime;


        if (m_HomingFactor > 1000 && move.magnitude > Vector3.Distance(m_Target.position - new Vector3(0, m_Target.position.y, 0), b.transform.position - new Vector3(0, b.transform.position.y, 0)))
        {
            move = m_Target.position - b.transform.position;
            move.y = 0;
        }

        b.MoveBy(move);
    }

}

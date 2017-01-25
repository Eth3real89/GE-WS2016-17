using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStraightMovement : BulletMovement {

    public float m_Speed;

    public override void HandleMovement(BulletBehaviour b)
    {
        b.transform.position += b.transform.forward * m_Speed * Time.deltaTime;
    }

}

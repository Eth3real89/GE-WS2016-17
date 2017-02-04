using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAbsoluteHomingMovement : BulletMovement {

    public Transform m_Target;

    public override void HandleMovement(BulletBehaviour b)
    {
        Vector3 diff = m_Target.transform.position - b.transform.position;
        b.MoveBy(diff);
    }
}

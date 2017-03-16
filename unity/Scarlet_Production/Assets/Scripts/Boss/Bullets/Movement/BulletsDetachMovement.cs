using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsDetachMovement : BulletMovement {

    protected bool m_First;

    public override void HandleMovement(BulletBehaviour b)
    {
        if (m_First)
            b.transform.parent = null;

        m_First = false;
    }

}

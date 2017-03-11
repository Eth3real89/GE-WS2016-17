using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDownwardsMovement : BulletStraightMovement {

    public float m_DownAngle;
    public float m_MinHeight;

    protected bool m_First = true;

    public override void HandleMovement(BulletBehaviour b)
    {
        if (m_First)
        {
            b.transform.Rotate(b.transform.right, m_DownAngle);
            m_First = false;
        }

        Vector3 moveBy = b.transform.forward * m_Speed * Time.deltaTime;

        if ((b.transform.position + moveBy).y < m_MinHeight)
        {
            moveBy.y = m_MinHeight - b.transform.position.y;
        }

        b.MoveBy(moveBy);
    }

}

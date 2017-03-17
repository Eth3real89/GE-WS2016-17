using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMoveDownThenHoming : BulletHomingMovement {

    protected float m_MinYHeight = 0.5f;

    public override void HandleMovement(BulletBehaviour b)
    {
        if (b.transform.position.y > m_MinYHeight)
        {
            b.transform.position = new Vector3(b.transform.position.x, 
                b.transform.position.y - Mathf.Min(m_Speed * Time.deltaTime, b.transform.position.y - m_MinYHeight),
                b.transform.position.z);
        }
        else
        {
            base.HandleMovement(b);
        }
    }

}

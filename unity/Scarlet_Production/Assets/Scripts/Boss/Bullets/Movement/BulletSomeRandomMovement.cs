using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSomeRandomMovement : BulletStraightMovement {

    private bool m_InitialMovement = false;

    public override void HandleMovement(BulletBehaviour b)
    {
        if (!m_InitialMovement)
        {
            m_Speed += Random.Range(0.1f, m_Speed);
            b.transform.rotation = Quaternion.Euler(0, b.transform.rotation.eulerAngles.y + Random.Range(-15f, 15f), 0);

            m_InitialMovement = true;
        }

        base.HandleMovement(b);
    }

}

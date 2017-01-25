using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BulletBehaviour
{
    public override void Kill()
    {
        m_KillBullet = true;
        Destroy(this.gameObject);
    }

    public override void MoveBy(Vector3 movement)
    {
        transform.position += movement;
    }
}

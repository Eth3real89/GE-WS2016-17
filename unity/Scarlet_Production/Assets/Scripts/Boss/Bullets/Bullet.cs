using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BulletBehaviour, BulletDamageTrigger.BulletDamageCallback
{

    public BulletDamage m_Damage;
    public BulletDamageTrigger m_DamageTrigger;

    public override void Launch(BulletCallbacks callbacks)
    {
        base.Launch(callbacks);
        m_DamageTrigger.m_Callback = this;
        m_DamageTrigger.m_Active = true;
    }

    public override void Kill()
    {
        m_KillBullet = true;
        if (this.gameObject != null)
            Destroy(this.gameObject);
    }

    public override void MoveBy(Vector3 movement)
    {
        transform.position += movement;
    }

    public void OnBulletCollision(Collider other)
    {

    }

    public void OnScarletCollidesWithBullet(GameObject scarlet)
    {
        m_DamageTrigger.m_Active = false;
        m_BulletCallbacks.OnBulletHitTarget(this, scarlet);

        PlayerHittable hittable = scarlet.GetComponentInChildren<PlayerHittable>();
        if (hittable != null)
        {
            hittable.Hit(m_Damage);
        }
    }
}

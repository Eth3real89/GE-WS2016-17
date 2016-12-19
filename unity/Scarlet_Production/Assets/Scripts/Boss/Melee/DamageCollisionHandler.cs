using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DamageCollisionHandler {

    void HandleCollision(Collider other);
}

public class DefaultCollisionHandler : DamageCollisionHandler
{
    private Damage m_Damage;

    public DefaultCollisionHandler(Damage damage)
    {
        this.m_Damage = damage;
    }

    public void HandleCollision(Collider other)
    {
        if (!m_Damage.m_Active)
            return;

        Hittable hittable = other.GetComponentInChildren<Hittable>();
        if (hittable != null && hittable is PlayerHittable)
        {
            hittable.Hit(this.m_Damage);
            m_Damage.m_Active = false;
        }
    }
}

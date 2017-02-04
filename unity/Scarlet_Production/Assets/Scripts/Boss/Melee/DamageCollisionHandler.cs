using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DamageCollisionHandler {

    void HandleScarletCollision(Collider other);
    void HandleCollision(Collider other, bool initialCollision);
    void HandleScarletLeave(Collider other);
}

public class DefaultCollisionHandler : DamageCollisionHandler
{
    private Damage m_Damage;

    public DefaultCollisionHandler(Damage damage)
    {
        this.m_Damage = damage;
    }

    public void HandleCollision(Collider other, bool initialCollision)
    {
        return;
    }

    public void SetDamageCallbacks(Damage.DamageCallback callback)
    {
        this.m_Damage.m_Callback = callback;
    }

    public void SetDamageBlockable(Damage.BlockableType blockableType)
    {
        this.m_Damage.m_BlockType = blockableType;
    }

    public void HandleScarletCollision(Collider other)
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

    public void HandleScarletLeave(Collider other)
    {
    }
}

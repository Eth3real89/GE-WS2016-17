using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTargetDamage : AEAttackDamage {

    private DefaultCollisionHandler m_CollisionHandler;

    public override void Activate()
    {
        base.Activate();
        m_CollisionHandler = new DefaultCollisionHandler(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_Active)
            return;

        Hittable hittable = other.GetComponentInChildren<Hittable>();
        if (hittable != null && hittable is PlayerHittable)
        {
            m_CollisionHandler.HandleScarletCollision(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!m_Active)
            return;

        Hittable hittable = other.GetComponentInChildren<Hittable>();
        if (hittable != null && hittable is PlayerHittable)
        {
            m_CollisionHandler.HandleScarletLeave(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!m_Active)
            return;

        Hittable hittable = other.GetComponentInChildren<Hittable>();
        if (hittable != null && hittable is PlayerHittable)
        {
            m_CollisionHandler.HandleScarletCollision(other);
        }
    }

}

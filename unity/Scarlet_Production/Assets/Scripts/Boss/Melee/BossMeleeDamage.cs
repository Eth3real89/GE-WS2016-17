using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMeleeDamage : Damage {

    public interface CollisionHandler
    {
        void HandleCollision(Collider other);
    }

    public CollisionHandler m_CollisionHandler;
    public bool m_Active;

    public override bool Blockable()
    {
        return true;
    }

    public override float DamageAmount()
    {
        return 30f;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (m_Active && m_CollisionHandler != null)
        {
            m_CollisionHandler.HandleCollision(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_Active && m_CollisionHandler != null)
        {
            m_CollisionHandler.HandleCollision(other);
        }
    }

    public class DefaultCollisionHandler : CollisionHandler
    {
        private BossMeleeDamage m_Damage;

        public DefaultCollisionHandler(BossMeleeDamage damage)
        {
            this.m_Damage = damage;
        }

        public void HandleCollision(Collider other)
        {
            if (!m_Damage.enabled)
                return;

            Hittable hittable = other.GetComponentInChildren<Hittable>();
            if (hittable != null)
            {
                hittable.hit(this.m_Damage);
                m_Damage.enabled = false;
            }
        }
    }

}

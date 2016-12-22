using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungeTrigger : Damage
{
    public DamageCollisionHandler m_CollisionHandler;

    public float m_Damage = 45f;

    public override bool Blockable()
    {
        return false;
    }

    public override float DamageAmount()
    {
        return m_Damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Active && m_CollisionHandler != null)
        {
            m_CollisionHandler.HandleScarletCollision(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_Active && m_CollisionHandler != null)
        {
            m_CollisionHandler.HandleScarletCollision(other);
        }
    }
}

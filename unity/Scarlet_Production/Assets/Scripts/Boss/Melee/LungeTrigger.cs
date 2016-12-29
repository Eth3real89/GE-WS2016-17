using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungeTrigger : Damage
{
    public DamageCollisionHandler m_CollisionHandler;

    public float m_Damage = 45f;
    public bool m_Blockable = true;

    public override bool Blockable()
    {
        return m_Blockable;
    }

    public override float DamageAmount()
    {
        return m_Damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Active && m_CollisionHandler != null && other.GetComponentInChildren<PlayerManager>() != null)
        {
            m_CollisionHandler.HandleScarletCollision(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_Active && m_CollisionHandler != null && other.GetComponentInChildren<PlayerManager>() != null)
        {
            m_CollisionHandler.HandleScarletCollision(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_Active && m_CollisionHandler != null && other.GetComponentInChildren<PlayerManager>() != null)
        {
            m_CollisionHandler.HandleScarletLeave(other);
        }
    }
}

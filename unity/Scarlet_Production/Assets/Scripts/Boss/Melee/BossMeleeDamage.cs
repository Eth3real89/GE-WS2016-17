using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMeleeDamage : Damage {

    public DamageCollisionHandler m_CollisionHandler;
    public BlockableType m_Blockable = BlockableType.Parry;

    public float m_Amount = 30f;

    public override BlockableType Blockable()
    {
        return m_Blockable;
    }

    private void Start()
    {
    }

    public override float DamageAmount()
    {
        return m_Amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTrigger(other, true);
    }

    private void OnTriggerStay(Collider other)
    {
        OnTrigger(other, false);
    }

    private void OnTrigger(Collider other, bool initialCollision)
    {
        if (m_Active && m_CollisionHandler != null && other.GetComponentInChildren<PlayerManager>() != null)
        {
            m_CollisionHandler.HandleScarletCollision(other);
        }
        else if
        (m_Active && m_CollisionHandler != null)
        {
            m_CollisionHandler.HandleCollision(other, initialCollision);
        }
    }

}

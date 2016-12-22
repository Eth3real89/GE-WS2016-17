using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMeleeDamage : Damage {

    public DamageCollisionHandler m_CollisionHandler;

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

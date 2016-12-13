using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : Damage {

    public bool m_Active;
    public float m_Damage;

    public override bool Blockable()
    {
        return true;
    }

    public override float DamageAmount()
    {
        return m_Damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckHit(other);
    }


    private void CheckHit(Collider other)
    {
        if (m_Active)
        {
            Hittable hittable = other.gameObject.GetComponentInChildren<Hittable>();
            if (hittable != null)
            {
                hittable.hit(this);
                m_Active = false;
            }
        }
    }
}

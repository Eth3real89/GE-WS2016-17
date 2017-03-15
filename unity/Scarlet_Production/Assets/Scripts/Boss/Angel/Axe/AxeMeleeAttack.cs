using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeMeleeAttack : AngelMeleeAttack
{
    public override void HandleCollision(Collider other, bool initialCollision)
    {
    }

    public override void HandleScarletLeave(Collider other)
    {
    }

    public override void OnBlockDamage()
    {
        m_Damage.m_Active = false;
    }

    public override void OnParryDamage()
    {
        m_Damage.m_Active = false;
    }
}

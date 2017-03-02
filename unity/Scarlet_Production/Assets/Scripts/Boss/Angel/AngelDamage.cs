using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelDamage : Damage {

    public float m_DamageAmount = 1f;

    public virtual void Activate()
    {
        m_Active = true;
    }

    public override BlockableType Blockable()
    {
        return m_BlockType;
    }

    public override float DamageAmount()
    {
        return m_DamageAmount;
    }
}

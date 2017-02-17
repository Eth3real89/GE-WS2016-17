using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorFairyParryDamage : Damage {

    public float m_DamageAmount;

    public override BlockableType Blockable()
    {
        return m_BlockType;
    }

    public override float DamageAmount()
    {
        return m_DamageAmount;
    }
}

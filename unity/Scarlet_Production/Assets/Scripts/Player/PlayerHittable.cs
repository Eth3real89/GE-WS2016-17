using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHittable : MonoBehaviour, Hittable {

    public CharacterHealth m_Health;
    private HitInterject m_Interject;

    public void Hit(Damage damage)
    {
        if (!m_Interject.OnHit(damage))
        {
            m_Health.m_CurrentHealth -= damage.DamageAmount();
            damage.OnSuccessfulHit();
        }
    }

    public void RegisterInterject(HitInterject interject)
    {
        m_Interject = interject;
    }
}

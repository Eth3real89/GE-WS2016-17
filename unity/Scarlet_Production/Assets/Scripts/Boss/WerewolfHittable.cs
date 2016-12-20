using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfHittable : MonoBehaviour, Hittable {

    public CharacterHealth m_Health;
    private HitInterject m_Interject;

    public OnHitSignal m_OnHitSignal;

    public void Hit(Damage damage)
    {
        if (m_Interject == null || !m_Interject.OnHit(damage))
        {
            m_Health.m_CurrentHealth -= damage.DamageAmount();
            damage.OnSuccessfulHit();

            if (m_OnHitSignal != null)
                m_OnHitSignal.OnHit();
        }
    }

    public void RegisterInterject(HitInterject interject)
    {
        m_Interject = interject;
    }
}

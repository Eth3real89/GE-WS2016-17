using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHittable : MonoBehaviour, Hittable {

    public CharacterHealth m_Health;

    public void hit(Damage damage)
    {
        m_Health.m_CurrentHealth -= damage.DamageAmount();
    }
}

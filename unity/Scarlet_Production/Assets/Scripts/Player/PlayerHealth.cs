using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : CharacterHealth {

    protected override void Start()
    {
        m_CurrentHealth = PlayerPrefs.GetFloat("MaxHP", m_MaxHealth);
        m_MaxHealth = m_CurrentHealth;
        m_HealthStart = m_CurrentHealth;
        m_HealthOld = m_CurrentHealth;
    }

}

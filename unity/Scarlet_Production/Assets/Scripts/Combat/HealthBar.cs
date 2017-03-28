using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public Slider m_HealthBar;
    public Slider m_LossBar;

    public GameObject m_Character;
    private CharacterHealth m_Health;

    private Image m_ImgHealthLoss;
   
    private float m_ElapsedTime;
    private float m_LastDmg;

	// Use this for initialization
	void Start () {
        m_Health = m_Character.GetComponentInChildren<CharacterHealth>();
        m_LastDmg = 0;
    }
	
	// Update is called once per frame
	void Update () {
        float damage = DetermineDamage();
        CalculateHealthBar(damage);

        if (damage == 0 || damage != m_LastDmg)
        {
            m_ElapsedTime = 0;
        }
        else
        {
            m_ElapsedTime += Time.deltaTime;

            if (m_ElapsedTime >= 1)
            {
                m_Health.m_HealthOld = m_Health.m_CurrentHealth;
            }
        }

        m_LastDmg = damage;
    }

    private float DetermineDamage()
    {
        return m_Health.m_HealthOld - m_Health.m_CurrentHealth;
    }

    private void CalculateHealthBar(float damage)
    {
        m_LossBar.value = m_Health.m_HealthOld / m_Health.m_MaxHealth;
        m_HealthBar.value = m_Health.m_CurrentHealth / m_Health.m_MaxHealth;
    }
}

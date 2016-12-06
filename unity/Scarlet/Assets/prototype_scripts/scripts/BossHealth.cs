using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour {

    public float m_MaxHealth;
    public float m_StartHealth;

    private float m_CurrentHealth;

    public bool m_Invincible = false;

	// Use this for initialization
	void Start () {
        m_CurrentHealth = m_StartHealth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TakeDamage(float howMuch)
    {
        if (!m_Invincible)
            m_CurrentHealth = Mathf.Max(0, m_CurrentHealth - howMuch);
    }

    public float GetBossHealth() 
    {
      return m_CurrentHealth;
    }

    public float GetMaxBossHealth()
    {
        return m_MaxHealth;
    }
}

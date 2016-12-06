using UnityEngine;
using System.Collections;

public class PlayerShield : MonoBehaviour {

    public float m_ActivateAfterSeconds = 10.0f;
    public float m_ShieldStrength = 30f;

    public bool m_ShieldActive = false;

    private float m_CurrentShieldStrength = 0f;

    private IEnumerator m_ShieldRepairCountdown;

	// Use this for initialization
	void Start () {
        m_ShieldRepairCountdown = ActivateShieldAfterWaiting();
        StartCoroutine(m_ShieldRepairCountdown);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public float OnPlayerTakeDamage(float damage)
    {
        if (m_ShieldActive)
        {
            damage = Mathf.Max(0, damage - m_CurrentShieldStrength);
            DisableShield();
        }

        if (m_ShieldRepairCountdown != null)
        {
            StopCoroutine(m_ShieldRepairCountdown);
        }
    
        m_ShieldRepairCountdown = ActivateShieldAfterWaiting();
        StartCoroutine(m_ShieldRepairCountdown);

        return damage;
    }

    private IEnumerator ActivateShieldAfterWaiting()
    {
        yield return new WaitForSeconds(m_ActivateAfterSeconds);

        ActivateShield();
    }

    private void DisableShield()
    {
        m_ShieldActive = false;
        m_CurrentShieldStrength = 0;

        GetComponent<Renderer>().enabled = false;
    }

    private void ActivateShield()
    {
        m_ShieldActive = true;
        m_CurrentShieldStrength = m_ShieldStrength;

        GetComponent<Renderer>().enabled = true;
    }
}

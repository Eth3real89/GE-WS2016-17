using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : MonoBehaviour {

    public float m_MaxHealth = 100f;
    public float m_HealthStart = 100f;

    /// <summary>
    /// Helper variable to be used by health bars.
    ///  will be set to m_HealthStart at Start (other values will be overriden).
    /// </summary>
    public float m_HealthOld;

    /// <summary>
    ///  will be set to m_HealthStart at Start (other values will be overriden).
    /// </summary>
    public float m_CurrentHealth;

	// Use this for initialization
	protected virtual void Start () {
        m_HealthOld = m_HealthStart;
        m_CurrentHealth = m_HealthStart;	
	}
	
}

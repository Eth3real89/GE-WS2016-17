using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour, AttackCombo.ComboCallback {

    public GameObject m_Scarlet;

    /// <summary>
    /// 
    /// </summary>
    public AttackCombo[] m_Combos;

	// Use this for initialization
	void Start () {
		if (m_Combos.Length > 0)
        {
            m_Combos[0].LaunchCombo();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnComboStart(AttackCombo combo)
    {
    }

    public void OnComboEnd(AttackCombo combo)
    {
    }

    public void OnActivateParry(AttackCombo combo)
    {
    }

    public void OnInterruptCombo(AttackCombo combo)
    {
    }
}

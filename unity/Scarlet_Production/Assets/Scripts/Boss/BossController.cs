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

    private int m_CurrentComboIndex;

    private IEnumerator m_NextComboTimer;

	// Use this for initialization
	void Start () {
        foreach(AttackCombo combo in m_Combos)
        {
            combo.m_Callback = this;
        }

        m_CurrentComboIndex = 0;

        StartCoroutine(StartAfterDelay());
	}

    private IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

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
        m_NextComboTimer = StartNextComboAfter(combo.m_TimeAfterCombo);
        StartCoroutine(m_NextComboTimer);
    }

    private IEnumerator StartNextComboAfter(float time)
    {
        yield return new WaitForSeconds(time);

        m_CurrentComboIndex++;
        if (m_CurrentComboIndex >= m_Combos.Length)
            m_CurrentComboIndex = 0;

        m_Combos[m_CurrentComboIndex].LaunchCombo();
    }

    public void OnActivateParry(AttackCombo combo)
    {
        // @todo maybe wait a little longer or do something special?
        OnComboEnd(combo);
    }

    public void OnInterruptCombo(AttackCombo combo)
    {
        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        m_NextComboTimer = StartNextComboAfter(combo.m_TimeAfterCombo);
        StartCoroutine(m_NextComboTimer);
    }
}

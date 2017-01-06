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

    protected int m_CurrentComboIndex;

    protected IEnumerator m_NextComboTimer;

    protected AttackCombo m_ActiveCombo;

	// Use this for initialization
	protected void Start ()
    {
/*        RegisterComboCallback();

        m_CurrentComboIndex = 0;

        StartCoroutine(StartAfterDelay()); */
    }

    protected void RegisterComboCallback()
    {
        foreach (AttackCombo combo in m_Combos)
        {
            combo.m_Callback = this;
        }
    }

    protected IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (m_Combos.Length > 0)
        {
            m_Combos[0].LaunchCombo();
        }
    }

    public virtual void OnComboStart(AttackCombo combo)
    {
        if (m_ActiveCombo != null)
            combo.CancelCombo();
        else
            m_ActiveCombo = combo;
    }

    public virtual void OnComboEnd(AttackCombo combo)
    {
        m_ActiveCombo = null;

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

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

    public virtual void OnActivateParry(AttackCombo combo)
    {
        // @todo maybe wait a little longer or do something special?
        OnComboEnd(combo);
    }

    public virtual void OnInterruptCombo(AttackCombo combo)
    {
        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        m_NextComboTimer = StartNextComboAfter(combo.m_TimeAfterCombo);
        StartCoroutine(m_NextComboTimer);
    }
}

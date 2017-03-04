using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealCommand : PlayerCommand {

    public int m_MaxHealthPotions = 3;

    public int m_NumHealthPotions
    {
        set
        {
            _NumHealthPotions = Math.Min(m_MaxHealthPotions, value);
            if (m_NumPotionListener != null || (m_NumPotionListenerObject != null && (m_NumPotionListener = m_NumPotionListenerObject.GetComponent<NumPotionListener>()) != null))
                m_NumPotionListener.OnNumberOfPotionsUpdated(_NumHealthPotions);
        }

        get
        {
            return _NumHealthPotions;
        }
    }

    public float m_MaxHealthGain = 33f;

    public GameObject m_NumPotionListenerObject;
    private NumPotionListener m_NumPotionListener;

    public CharacterHealth m_PlayerHealth;

    private int _NumHealthPotions;

    private void Start()
    {
        m_CommandName = "Heal";
    }

    public override void InitTrigger()
    {
        m_CommandName = "Heal";
        m_Trigger = new PressAxisTrigger(this, "Heal");
    }

    public override void TriggerCommand()
    {
        if (m_NumHealthPotions > 0)
            DoHeal();
    }

    private void DoHeal()
    {
        m_Callback.OnCommandStart(m_CommandName, this);
        m_PlayerHealth.m_CurrentHealth = Mathf.Min(m_PlayerHealth.m_CurrentHealth + m_MaxHealthGain, m_PlayerHealth.m_MaxHealth);
        m_NumHealthPotions -= 1;

        m_Callback.OnCommandEnd(m_CommandName, this);
    }

    public void ResetPotions()
    {
        m_NumHealthPotions = m_MaxHealthPotions;
    }

    // heal cannot be cancelled.
    public override void CancelDelay()
    {
    }

    public interface NumPotionListener
    {
        void OnNumberOfPotionsUpdated(int num);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, Damage.DamageCallback {

    public enum State
    {
        Invincible,
        Stunned
    };

    public int m_HealthPotions;

    public Damage m_PlayerDamage;
    public PlayerStaggerCommand m_StaggerCommand;

    // Use this for initialization
    void Start()
    {
        if (m_PlayerDamage != null)
        {
            m_PlayerDamage.m_Callback = this;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnParryDamage()
    {
        if (m_StaggerCommand != null)
        {
            m_StaggerCommand.TriggerCommand();
        }
    }

    public void OnBlockDamage()
    {
    }

    public void OnSuccessfulHit()
    {
    }
}

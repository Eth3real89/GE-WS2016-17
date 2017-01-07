using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The PlayerManager contains the player's status and
/// (@todo later) their abilities etc.
/// </summary>
public class PlayerManager : MonoBehaviour, Damage.DamageCallback, LightField.LightFieldResponder, ClimbableArea.ClimbAreaResponder
{
    public enum State
    {
        Invincible,
        Stunned
    };

    public int m_StartHealthPotions;
    public PlayerHealCommand m_HealCommand;

    public Damage m_PlayerDamage;
    public PlayerStaggerCommand m_StaggerCommand;

    public PlayerLightEffects m_LightEffects;
    public PlayerClimbingHandler m_ClimbingHandler;

    private bool m_IsClimbing;

    // Use this for initialization
    void Start()
    {
        if (m_PlayerDamage != null)
        {
            m_PlayerDamage.m_Callback = this;
        }

        if (m_HealCommand != null)
        {
            m_HealCommand.m_NumHealthPotions = m_StartHealthPotions;
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

    public void OnEnterLightField(LightField.LightFieldClass lightFieldClass, Vector3 retreatDirection)
    {
        if (m_LightEffects != null && lightFieldClass == LightField.LightFieldClass.Regular)
            m_LightEffects.OnPlayerEnterLight();
        if (m_LightEffects != null && lightFieldClass == LightField.LightFieldClass.Strong)
            m_LightEffects.OnPlayerEnterStrongLight(retreatDirection);
    }

    public void OnStayInLightField(LightField.LightFieldClass lightFieldClass)
    {
    }

    public void OnExitLightField(LightField.LightFieldClass lightFieldClass)
    {
        if (m_LightEffects != null && lightFieldClass == LightField.LightFieldClass.Regular)
            m_LightEffects.OnPlayerExitsLight();
        if (m_LightEffects != null && lightFieldClass == LightField.LightFieldClass.Strong)
            m_LightEffects.OnPlayerExitStrongLight();
    }

    public void OnEnterClimbArea()
    {
    }

    public void OnStayInClimbArea()
    {
        if (Input.GetButtonDown("Dash"))
        {
            if (!m_IsClimbing)
            {
                m_ClimbingHandler.StartClimbing();
                m_IsClimbing = true;
            }
            else
            {
                m_ClimbingHandler.StopClimbing();
                m_IsClimbing = false;
            }
        }

        if (m_IsClimbing && Input.GetAxis("Vertical") < 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.1f))
            {
                if (hit.collider.tag == "Ground")
                {
                    m_ClimbingHandler.StopClimbing();
                    m_IsClimbing = false;
                }
            }
        }
    }

    public void OnExitClimbArea()
    {
    }
}

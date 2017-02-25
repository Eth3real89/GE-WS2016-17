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
    public PlayerControls m_PlayerControls;

    public bool m_BeforeAuraPickUp;

    private bool m_IsClimbing;
    private List<GameObject> m_TrackLightFields;
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private GameObject m_Collectible;

    // Use this for initialization
    void Start()
    {
        m_TrackLightFields = new List<GameObject>();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collectible = GameObject.Find("Collectible");

        if (m_PlayerDamage != null)
        {
            m_PlayerDamage.m_Callback = this;
        }

        if (m_HealCommand != null)
        {
            m_HealCommand.m_NumHealthPotions = m_StartHealthPotions;
        }
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

    public void OnEnterLightField(GameObject lightField)
    {
        if ((m_Collectible == null || m_Collectible.activeSelf == false) && m_BeforeAuraPickUp)
        {
            m_BeforeAuraPickUp = false;
        }
        Vector3 retreatDirection = lightField.GetComponent<LightField>().GetVectorFromDirection(m_Rigidbody.velocity);
        LightField.LightFieldClass lightFieldClass = lightField.GetComponent<LightField>().m_Class;
        if (!m_TrackLightFields.Contains(lightField))
            m_TrackLightFields.Add(lightField);

        if(!m_BeforeAuraPickUp)
        {
            transform.Find("DarknessAura").gameObject.SetActive(false);
        }
        if (m_LightEffects != null && lightFieldClass == LightField.LightFieldClass.Regular)
            m_LightEffects.OnPlayerEnterLight();
        if (m_LightEffects != null && lightFieldClass == LightField.LightFieldClass.Strong)
            m_LightEffects.OnPlayerEnterStrongLight(retreatDirection);
    }

    public void OnStayInLightField(LightField.LightFieldClass lightFieldClass)
    {
    }

    public void OnExitLightField(GameObject lightField)
    {
        if ((m_Collectible == null || m_Collectible.activeSelf == false) && m_BeforeAuraPickUp)
        {
            m_BeforeAuraPickUp = false;
        }
        LightField.LightFieldClass lightFieldClass = lightField.GetComponent<LightField>().m_Class;
        if (m_TrackLightFields.Contains(lightField))
            m_TrackLightFields.Remove(lightField);

        if (!m_BeforeAuraPickUp)
        {
            transform.Find("DarknessAura").gameObject.SetActive(true);
        }

        if (m_LightEffects != null && lightFieldClass == LightField.LightFieldClass.Regular)
            m_LightEffects.OnPlayerExitsLight();
        if (m_LightEffects != null && lightFieldClass == LightField.LightFieldClass.Strong)
            m_LightEffects.OnPlayerExitStrongLight();
        if (m_TrackLightFields.Count > 0)
        {
            OnEnterLightField(m_TrackLightFields[0]);
        }
    }

    public void OnEnterClimbArea()
    {
        if (Input.GetButton("Dash"))
        {
            if (!m_IsClimbing)
            {
                m_ClimbingHandler.StartClimbing();
                m_IsClimbing = true;
            }
        }
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
    }

    public void OnExitClimbArea()
    {
        m_ClimbingHandler.StopClimbing();
        m_IsClimbing = false;
    }

    public void OnHitGround(string groundTag)
    {
        if (m_IsClimbing && groundTag == "Ground")
        {
            m_ClimbingHandler.StopClimbing();
            m_IsClimbing = false;
        }

        m_PlayerControls.EnableAllCommands();
        m_Animator.SetBool("IsFalling", false);
        m_Rigidbody.velocity = Vector3.zero;
    }

    public void OnLoseGround()
    {
        if (m_IsClimbing)
            return;

        m_PlayerControls.DisableAllCommands();
        m_Animator.SetBool("IsFalling", true);
    }
}

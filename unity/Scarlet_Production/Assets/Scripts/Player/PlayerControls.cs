using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour, PlayerCommandCallback {

    public PlayerCommand[] m_PlayerCommands;

    private PlayerAttackCommand m_AttackCommand;
    private PlayerDashCommand m_DashCommand;
    private PlayerHealCommand m_HealCommand;
    private PlayerMoveCommand m_MoveCommand;
    private PlayerParryCommand m_ParryCommand;
    private PlayerStaggerCommand m_StaggerCommand;

    void Start () {
        m_PlayerCommands = GetComponentsInChildren<PlayerCommand>();
        foreach(PlayerCommand command in m_PlayerCommands)
        {
            command.Init(this, gameObject, GetComponentInChildren<Animator>());
        }

        ReferenceCommands();
	}

    private void ReferenceCommands()
    {
        m_AttackCommand = GetComponentInChildren<PlayerAttackCommand>();
        m_DashCommand = GetComponentInChildren<PlayerDashCommand>();
        m_HealCommand = GetComponentInChildren<PlayerHealCommand>();
        m_MoveCommand = GetComponentInChildren<PlayerMoveCommand>();
        m_ParryCommand = GetComponentInChildren<PlayerParryCommand>();
        m_StaggerCommand = GetComponentInChildren<PlayerStaggerCommand>();
    }
	
	void Update () {

    }

    public void OnCommandEnd(string commandName, PlayerCommand command)
    {
        if (command == m_HealCommand)
            return;

        // so far, there is no "switch case" thing here; if a command was not interrupted
        // (by, say, staggering), then it was the only one that was active (with the exception of healing)
        // and all other commands become available again.
        EnableCommands(m_AttackCommand, m_DashCommand, m_HealCommand, 
                       m_ParryCommand, m_MoveCommand, m_StaggerCommand);
    }

    public void OnCommandStart(string commandName, PlayerCommand command)
    {
        if (command == m_AttackCommand)
            DisableCommands(m_AttackCommand, m_DashCommand, m_HealCommand, m_ParryCommand, m_MoveCommand);
        else if (command == m_DashCommand)
            DisableCommands(m_AttackCommand, m_ParryCommand, m_MoveCommand);
        else if (command == m_ParryCommand)
            DisableCommands(m_AttackCommand, m_ParryCommand, m_DashCommand, m_MoveCommand);
    }

    private void DisableCommands(params PlayerCommand[] commands)
    {
        foreach (PlayerCommand command in commands)
            DisableCommand(command);
    }

    private void EnableCommands(params PlayerCommand[] commands)
    {
        foreach (PlayerCommand command in commands)
            EnableCommand(command);
    }

    private void DisableCommand(PlayerCommand command)
    {
        if (command != null)
            command.m_Active = false;
    }

    private void EnableCommand(PlayerCommand command)
    {
        if (command != null)
            command.m_Active = true;
    }
}

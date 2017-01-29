using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour, PlayerCommandCallback, PlayerParryCommand.ParryCallback, PlayerAttackCommand.AttackCallback
{

    public PlayerCommand[] m_PlayerCommands;

    private PlayerAttackCommand m_AttackCommand;
    private PlayerDashCommand m_DashCommand;
    private PlayerHealCommand m_HealCommand;
    private PlayerMoveCommand m_MoveCommand;
    private PlayerParryCommand m_ParryCommand;
    private PlayerStaggerCommand m_StaggerCommand;
    private PlayerInteractionCommand m_InteractionCommand;

    private PlayerCommand m_ActiveCommand;

    private List<PlayerCommand> m_LockedCommands;

    private IEnumerator m_DashDelayEnumerator;
    private IEnumerator m_SlowMoEnumerator;

    public void DisableAllCommands()
    {
        DisableCommands(m_PlayerCommands);
    }

    public void EnableAllCommands()
    {
        EnableCommands(m_PlayerCommands);
    }

    public void DisableAndLock(params PlayerCommand[] commands)
    {
        foreach (PlayerCommand c in commands)
        {
            if (m_ActiveCommand == c)
                m_ActiveCommand = null;

            c.CancelDelay();
            c.m_Active = false;
            if (!m_LockedCommands.Contains(c))
            {
                m_LockedCommands.Add(c);
            }
        }
    }

    public void EnableAndUnlock(params PlayerCommand[] commands)
    {
        foreach (PlayerCommand c in commands)
        {
            c.m_Active = true;
            if (m_LockedCommands.Contains(c))
            {
                m_LockedCommands.Remove(c);
            }
        }
    }

    void Start()
    {
        m_PlayerCommands = GetComponentsInChildren<PlayerCommand>();
        m_LockedCommands = new List<PlayerCommand>();

        foreach (PlayerCommand command in m_PlayerCommands)
        {
            command.Init(this, gameObject, GetComponentInChildren<Animator>());
            if (command.m_DefaultLocked)
            {
                DisableAndLock(command);
            }
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
        m_InteractionCommand = GetComponentInChildren<PlayerInteractionCommand>();

        if (m_ParryCommand != null)
            m_ParryCommand.m_ParryCallback = this;

        if (m_AttackCommand != null)
            m_AttackCommand.m_AttackCallback = this;
    }

    public void OnCommandEnd(string commandName, PlayerCommand command)
    {
        m_ActiveCommand = null;

        if (command == m_HealCommand)
        {
            return;
        }
        else if (command == m_DashCommand)
        {
            EnableCommands(m_AttackCommand, m_HealCommand, m_ParryCommand, m_MoveCommand, m_StaggerCommand);
            m_DashDelayEnumerator = EnableDashAfter(m_DashCommand.m_DashDelay);
            StartCoroutine(m_DashDelayEnumerator);
        }
        else
        {
            EnableCommands(m_AttackCommand, m_DashCommand, m_HealCommand,
                        m_ParryCommand, m_MoveCommand, m_StaggerCommand);
        }
    }

    public void OnCommandStart(string commandName, PlayerCommand command)
    {
        m_ActiveCommand = command;

        if (command == m_AttackCommand)
        {
            DisableCommands(m_AttackCommand, m_DashCommand, m_HealCommand, m_ParryCommand, m_MoveCommand);
            m_MoveCommand.StopMoving();
        }
        else if (command == m_DashCommand)
        {
            DisableCommands(m_AttackCommand, m_ParryCommand, m_MoveCommand, m_DashCommand);
            m_MoveCommand.StopMoving();
        }
        else if (command == m_ParryCommand)
        {
            DisableCommands(m_AttackCommand, m_ParryCommand, m_DashCommand, m_MoveCommand);
            m_MoveCommand.StopMoving();
        }
        else if (command == m_StaggerCommand)
        {
            CancelCommands(m_AttackCommand, m_ParryCommand, m_HealCommand, m_DashCommand, m_MoveCommand);
            DisableCommands(m_AttackCommand, m_ParryCommand, m_HealCommand, m_DashCommand, m_MoveCommand);
            m_MoveCommand.StopMoving();
        }
        else if (command = m_InteractionCommand)
        {
            CancelCommands(m_AttackCommand, m_ParryCommand, m_HealCommand, m_DashCommand, m_MoveCommand);
            DisableCommands(m_AttackCommand, m_ParryCommand, m_HealCommand, m_DashCommand, m_MoveCommand);
            m_MoveCommand.StopMoving();
        }
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

        if (command == m_DashCommand && m_DashDelayEnumerator != null)
            StopCoroutine(m_DashDelayEnumerator);
    }

    private void EnableCommand(PlayerCommand command)
    {
        if (command != null && !m_LockedCommands.Contains(command))
            command.m_Active = true;
    }

    private void CancelCommands(params PlayerCommand[] commands)
    {
        foreach (PlayerCommand command in commands)
            CancelCommand(command);
    }

    private void CancelCommand(PlayerCommand command)
    {
        if (command != null)
            command.CancelDelay();
    }

    private IEnumerator EnableDashAfter(float time)
    {
        yield return new WaitForSeconds(time);
        EnableCommand(m_DashCommand);
    }

    public void OnPerfectParry()
    {
        if (m_AttackCommand != null)
            m_AttackCommand.m_RiposteActive = true;

        SlowTime.Instance.StartSlowMo();

        m_SlowMoEnumerator = PerfectParryEnumerator();

        StartCoroutine(m_SlowMoEnumerator);

        CameraController.Instance.ZoomIn();
        DisableCommands(m_ParryCommand, m_MoveCommand, m_DashCommand);
    }

    private IEnumerator PerfectParryEnumerator()
    {
        yield return new WaitForSeconds(1 * SlowTime.Instance.m_SlowAmount);

        SlowTime.Instance.StopSlowMo();
        m_AttackCommand.m_RiposteActive = false;

        CameraController.Instance.ActivateDefaultCamera();

        // @todo need a better solution, e.g. make slow-mo longer if attack occurred & start a new timer
        // (these things need to be enabled, but not necessarily here!)
        if (m_ActiveCommand == null)
            EnableCommands(m_ParryCommand, m_MoveCommand, m_DashCommand);
    }

    public void OnBlock()
    {// do nothing special
    }

    public void OnParryFail()
    {
        if (m_StaggerCommand != null)
            m_StaggerCommand.TriggerCommand();
    }

    public void OnPlayerAttackParried()
    {
        if (m_ActiveCommand != null)
            m_ActiveCommand.CancelDelay();

        if (m_StaggerCommand != null)
            m_StaggerCommand.TriggerCommand();
    }

    public void OnPlayerAttackBlocked()
    {
    }

    public void OnPlayerActivateRiposte()
    {
        if (m_SlowMoEnumerator != null)
            StopCoroutine(m_SlowMoEnumerator);

        StartCoroutine(ResetAfterRiposte());
    }

    public IEnumerator ResetAfterRiposte()
    {
        yield return new WaitForSeconds(0.15f * SlowTime.Instance.m_SlowAmount);

        SlowTime.Instance.StopSlowMo();
        CameraController.Instance.ActivateDefaultCamera();
    }
}

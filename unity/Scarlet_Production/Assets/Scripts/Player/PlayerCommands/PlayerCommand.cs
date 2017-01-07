using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCommand : MonoBehaviour
{
    public enum CommandAvailability
    {
        Anywhere = -1, Exploration = 0, Combat = 1
    }

    public string m_CommandName;
    public string[] m_Tags;
    public CommandAvailability m_Availability;

    public bool m_Active;

    public PlayerCommandCallback m_Callback;
    public GameObject m_Scarlet;
    public Animator m_Animator;

    public CommandTrigger m_Trigger;

    public void Init(PlayerCommandCallback callback, GameObject scarlet, Animator animator)
    {
        m_Callback = callback;
        m_Scarlet = scarlet;
        m_Animator = animator;

        InitTrigger();
    }

    void Update()
    {
        if (m_Trigger != null)
            m_Trigger.Update();
    }

    public bool IsCommandAvailable()
    {
        if (m_Availability == CommandAvailability.Anywhere)
            return true;

        PlayerManager playerManager = m_Scarlet.GetComponentInChildren<PlayerManager>();
        PlayerManager.ControlMode currentControlMode = playerManager.m_ControlMode;
        return (int)m_Availability == (int)currentControlMode;
    }

    public abstract void InitTrigger();
    public abstract void TriggerCommand();
    public abstract void CancelDelay();

    public abstract class CommandTrigger
    {
        protected PlayerCommand m_Command;

        public CommandTrigger(PlayerCommand command)
        {
            this.m_Command = command;
        }

        public abstract void Update();
    }

    public class ConstantAxisTrigger : CommandTrigger
    {
        private string m_Axis;

        public ConstantAxisTrigger(PlayerCommand command, String axis) : base(command)
        {
            m_Axis = axis;
        }

        public override void Update()
        {
            if (!m_Command.m_Active && !m_Command.IsCommandAvailable())
                return;

            float axisValue = Input.GetAxis(m_Axis);
            if (axisValue > 0)
            {
                m_Command.TriggerCommand();
            }
        }
    }

    public class PressAxisTrigger : CommandTrigger
    {
        private string m_Axis;

        private float m_Pressed;

        public PressAxisTrigger(PlayerCommand command, String axis) : base(command)
        {
            m_Axis = axis;
        }

        public override void Update()
        {
            float pressed = Input.GetAxis(m_Axis);

            if (m_Command.m_Active && m_Command.IsCommandAvailable())
            {
                if (pressed > 0 && pressed != m_Pressed)
                {
                    m_Command.TriggerCommand();
                }
            }

            m_Pressed = pressed;
        }
    }
}

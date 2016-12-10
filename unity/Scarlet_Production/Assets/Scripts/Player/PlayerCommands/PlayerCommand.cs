using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCommand : MonoBehaviour {

    public string m_CommandName;
    public string[] m_Tags;

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
        m_Trigger.Update();
    }

    public abstract void InitTrigger();
    public abstract void TriggerCommand();

    public abstract class CommandTrigger
    {
        protected PlayerCommand m_Command;

        public CommandTrigger(PlayerCommand command)
        {
            this.m_Command = command;
        }

        public abstract void Update();
    }

    public class DefaultAxisTrigger : CommandTrigger
    {
        private string m_Axis;

        public DefaultAxisTrigger(PlayerCommand command, String axis) : base(command)
        {
            m_Axis = axis;
        }

        public override void Update()
        {
            if (!m_Command.m_Active)
                return;

            float axisValue = Input.GetAxis(m_Axis);
            if (axisValue > 0)
            {
                m_Command.TriggerCommand();
            }
        }
    }
}

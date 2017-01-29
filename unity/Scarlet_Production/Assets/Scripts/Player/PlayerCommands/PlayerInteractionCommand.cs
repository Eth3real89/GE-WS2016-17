using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionCommand : PlayerCommand
{
    public float m_InteractionTime;
    public float m_InteractionRange;
    public Transform m_Anchor;

    private float m_CurrentInteraction;

    private void Start()
    {
        m_CurrentInteraction = m_InteractionTime;
        m_CommandName = "Attack";
    }

    public override void InitTrigger()
    {
        m_CommandName = "Attack";
        m_Trigger = new PickUpTrigger(this);
    }

    public override void TriggerCommand()
    {
        bool isInteracting = false;
        RaycastHit[] hits = Physics.RaycastAll(m_Anchor.position, m_Anchor.forward, m_InteractionRange);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag == "PickUp")
            {
                m_Callback.OnCommandStart(m_CommandName, this);
                isInteracting = true;
                m_Animator.SetBool("IsInteracting", true);
                m_CurrentInteraction -= Time.deltaTime;
                if (m_CurrentInteraction < 0)
                {
                    hit.transform.GetComponent<Interactor>().Interact();
                }
            }
        }

        if (!isInteracting)
        {
            CancelDelay();
        }
    }

    // used for "Cancel Timer"
    public override void CancelDelay()
    {
        m_Callback.OnCommandEnd(m_CommandName, this);
        m_CurrentInteraction = m_InteractionTime;
        m_Animator.SetBool("IsInteracting", false);
    }

    private class PickUpTrigger : CommandTrigger
    {
        new PlayerInteractionCommand m_Command;

        public PickUpTrigger(PlayerInteractionCommand command) : base(command)
        {
            m_Command = command;
        }

        public override void Update()
        {
            if (!m_Command.m_Active || !m_Command.IsCommandAvailable())
                return;

            if (Input.GetButton(m_Command.m_CommandName))
            {
                m_Command.TriggerCommand();
            }

            if (Input.GetButtonUp(m_Command.m_CommandName))
            {
                m_Command.CancelDelay();
            }
        }
    }
}

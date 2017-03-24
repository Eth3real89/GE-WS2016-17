using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionCommand : PlayerCommand
{
    public float m_InteractionTime;
    public float m_InteractionRange;
    public Transform m_Anchor;
    public Interactor m_CurrentInteractor;

    private float m_CurrentInteraction;

    private void Start()
    {
        m_CurrentInteraction = m_InteractionTime;
    }

    public override void InitTrigger()
    {
        m_CommandName = "Attack";
        m_Trigger = new PickUpTrigger(this);
    }

    public override void TriggerCommand()
    {
        bool isInteracting = false;
        if (m_CurrentInteractor != null)
        {
            Vector3 target = m_CurrentInteractor.transform.position;
            target.y = GameObject.FindGameObjectWithTag("Player").transform.position.y;
            transform.parent.LookAt(target);
            m_Animator.SetBool("IsInteracting", m_CurrentInteractor.m_UseAnimation);
            m_Callback.OnCommandStart(m_CommandName, this);
            isInteracting = true;
            m_CurrentInteraction -= Time.deltaTime;
            if (m_CurrentInteractor.GetComponentInChildren<UIItemPickupController>() != null)
            {
                m_CurrentInteractor.GetComponentInChildren<UIItemPickupController>().UpdatePickup(m_InteractionTime - m_CurrentInteraction);
            }
            if (m_CurrentInteraction < 0)
            {
                if (m_CurrentInteractor!= null)
                    m_CurrentInteractor.Interact();
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
        if (m_CurrentInteractor != null && m_CurrentInteractor.GetComponentInChildren<UIItemPickupController>() != null)
        {
            m_CurrentInteractor.GetComponentInChildren<UIItemPickupController>().UpdatePickup(0.0f);
        }
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

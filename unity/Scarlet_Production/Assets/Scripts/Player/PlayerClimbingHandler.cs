using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingHandler : MonoBehaviour
{
    public PlayerControls m_PlayerControls;

    public PlayerSprintCommand m_SprintCommand;
    public PlayerMoveCommand m_MoveCommand;
    public PlayerClimbCommand m_ClimbCommand;

    private Animator m_Animator;
    private Rigidbody m_Rigidbody;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public void StartClimbing()
    {
        m_MoveCommand.TriggerManually(Vector3.forward);
        m_MoveCommand.StopMoving();
        m_Animator.SetBool("IsClimbing", true);
        // @Todo: find a better solution to avoid bumping of the wall, works for now
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        m_Rigidbody.useGravity = false;
        m_PlayerControls.DisableAllCommands();
        m_PlayerControls.DisableAndLock(m_MoveCommand, m_SprintCommand);
        m_PlayerControls.EnableAndUnlock(m_ClimbCommand);
    }

    public void StopClimbing()
    {
        m_Animator.SetBool("IsClimbing", false);
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        m_Rigidbody.useGravity = true;
        m_PlayerControls.EnableAllCommands();
        m_PlayerControls.EnableAndUnlock(m_MoveCommand, m_SprintCommand);
        m_PlayerControls.DisableAndLock(m_ClimbCommand);
    }
}

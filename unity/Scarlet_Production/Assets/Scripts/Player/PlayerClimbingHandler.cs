using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingHandler : MonoBehaviour
{
    public PlayerControls m_PlayerControls;

    public PlayerSprintCommand m_SprintCommand;
    public PlayerMoveCommand m_MoveCommand;
    public PlayerClimbCommand m_ClimbCommand;

    public ParticleSystem m_ClimbLeftHand;
    public ParticleSystem m_ClimbRightHand;

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

    public IEnumerator OnHandGrip(int handSide)
    {
        if(handSide == 0)
        {
            //Right side
            m_ClimbRightHand.Play();
            yield return new WaitForSeconds(0.3f);
            m_ClimbRightHand.Stop();
        }
        else
        {
            //Left side
            m_ClimbLeftHand.Play();
            yield return new WaitForSeconds(0.3f);
            m_ClimbLeftHand.Stop();
        }
    }
}

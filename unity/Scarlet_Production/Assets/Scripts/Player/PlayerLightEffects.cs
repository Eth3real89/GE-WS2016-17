using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightEffects : MonoBehaviour
{

    public PlayerControls m_PlayerControls;

    public PlayerDashCommand m_DashCommand;
    public PlayerAttackCommand m_AttackCommand;

    public PlayerMoveCommand m_MoveCommand;
    public float m_MovementSpeedInLight;
    private float m_RegularMovementSpeed;


    public void OnPlayerEnterLight()
    {
        m_RegularMovementSpeed = m_MoveCommand.m_RunSpeed;
        m_MoveCommand.m_RunSpeed = m_MovementSpeedInLight;

        m_PlayerControls.DisableAndLock(m_DashCommand, m_AttackCommand);
    }

    public void OnPlayerExitsLight()
    {
        m_MoveCommand.m_RunSpeed = m_RegularMovementSpeed;
        m_PlayerControls.EnableAndUnlock(m_DashCommand, m_AttackCommand);
    }

    public void OnPlayerEnterStrongLight(Vector3 retreatDirection)
    {
        m_RegularMovementSpeed = m_MoveCommand.m_RunSpeed;
        m_MoveCommand.m_RunSpeed = 0.3f;
        StartCoroutine(StartDelayedRetreat(retreatDirection));
    }

    public void OnPlayerExitStrongLight()
    {
        m_PlayerControls.EnableAllCommands();
        m_MoveCommand.m_RunSpeed = m_RegularMovementSpeed;
    }

    // let the player step into the light for a bit...
    IEnumerator StartDelayedRetreat(Vector3 retreatDirection)
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(Retreat(retreatDirection));
    }

    // ...and then get out of there
    IEnumerator Retreat(Vector3 retreatDirection)
    {
        m_PlayerControls.DisableAllCommands();
        while (m_MoveCommand.m_RunSpeed != m_RegularMovementSpeed)
        {
            m_MoveCommand.TriggerManually(retreatDirection);
            yield return null;
        }
    }
}

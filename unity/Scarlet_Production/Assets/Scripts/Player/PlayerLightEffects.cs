using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightEffects : MonoBehaviour
{
    public PlayerControls m_PlayerControls;

    public PlayerDashCommand m_DashCommand;
    public PlayerAttackCommand m_AttackCommand;
    public PlayerSprintCommand m_SprintCommand;

    public PlayerMoveCommand m_MoveCommand;
    public float m_MovementSpeedInLight;
    public float m_MovementSpeedInStrongLight;
    private float m_RegularMovementSpeed;

    private bool m_InsideStrongLight;

    public void OnPlayerEnterLight()
    {
        m_RegularMovementSpeed = m_MoveCommand.m_CurrentSpeed;
        m_MoveCommand.m_CurrentSpeed = m_MovementSpeedInLight;

        m_PlayerControls.DisableAndLock(m_DashCommand, m_AttackCommand, m_SprintCommand);
    }

    public void OnPlayerExitsLight()
    {
        m_MoveCommand.m_CurrentSpeed = m_RegularMovementSpeed;
        m_PlayerControls.EnableAndUnlock(m_DashCommand, m_AttackCommand, m_SprintCommand);
    }

    public void OnPlayerEnterStrongLight(Vector3 retreatDirection)
    {
        m_InsideStrongLight = true;
        m_RegularMovementSpeed = m_MoveCommand.m_CurrentSpeed;
        m_MoveCommand.m_CurrentSpeed = m_MovementSpeedInStrongLight;
        m_PlayerControls.DisableAndLock(m_DashCommand, m_AttackCommand, m_SprintCommand);
        StartCoroutine(StartDelayedRetreat(retreatDirection));
    }

    public void OnPlayerExitStrongLight()
    {
        m_PlayerControls.EnableAndUnlock(m_DashCommand, m_AttackCommand, m_SprintCommand);
        m_PlayerControls.EnableAllCommands();
        m_InsideStrongLight = false;
        m_MoveCommand.m_CurrentSpeed = m_RegularMovementSpeed;
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
        while (m_InsideStrongLight)
        {
            m_MoveCommand.TriggerManually(retreatDirection);
            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightEffects : MonoBehaviour {

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

    public void OnPlayerEnterStrongLight()
    {
        // @todo
    }

    public void OnPlayerExitStrongLight()
    {
        // @todo
    }

}

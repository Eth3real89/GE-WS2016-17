using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : GenericSingletonClass<LevelManager>
{
    public enum ControlMode
    {
        Exploration = 0,
        Combat = 1
    }

    public ControlMode m_ControlMode = ControlMode.Exploration;
    public GameObject m_Scarlet;

    private PlayerMoveCommand m_ScarletMoveCommand;

    void Start()
    {
        m_Scarlet = GameObject.FindGameObjectWithTag("Player");
        m_ScarletMoveCommand = m_Scarlet.GetComponentInChildren<PlayerMoveCommand>();
        SetupPlayerStats();
    }

    private void SetupPlayerStats()
    {
        if (m_ControlMode == ControlMode.Exploration)
            m_ScarletMoveCommand.m_CurrentSpeed = m_ScarletMoveCommand.m_WalkSpeed;
        if (m_ControlMode == ControlMode.Combat)
            m_ScarletMoveCommand.m_CurrentSpeed = m_ScarletMoveCommand.m_RunSpeedCombat;
    }
}

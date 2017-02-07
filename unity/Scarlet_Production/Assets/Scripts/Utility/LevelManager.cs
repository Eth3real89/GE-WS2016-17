using System.Collections;
using System;
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
    private Animator m_ScarletAnimator;

    void Start()
    {
        m_Scarlet = GameObject.FindGameObjectWithTag("Player");
        m_ScarletAnimator = m_Scarlet.GetComponent<Animator>();
        m_ScarletMoveCommand = m_Scarlet.GetComponentInChildren<PlayerMoveCommand>();
        SetupPlayerStats();
    }

    private void SetupPlayerStats()
    {
        m_ScarletAnimator.SetFloat("IsCombatMode", Convert.ToInt32(m_ControlMode == ControlMode.Combat));
        if (m_ControlMode == ControlMode.Exploration)
            m_ScarletMoveCommand.m_CurrentSpeed = m_ScarletMoveCommand.m_WalkSpeed;
        if (m_ControlMode == ControlMode.Combat)
            m_ScarletMoveCommand.m_CurrentSpeed = m_ScarletMoveCommand.m_RunSpeedCombat;
    }

    public void QuickLoadFix()
    {
        StartCoroutine(SetRunSpeedAfterWaiting());
    }

    private IEnumerator SetRunSpeedAfterWaiting()
    {
        yield return new WaitForSeconds(1f);

        GameObject m_Scarlet = GameObject.FindGameObjectWithTag("Player");
        PlayerMoveCommand moveCommand = m_Scarlet.GetComponentInChildren<PlayerMoveCommand>();
        if (moveCommand != null)
            moveCommand.m_CurrentSpeed = moveCommand.m_RunSpeedCombat;
    }
}

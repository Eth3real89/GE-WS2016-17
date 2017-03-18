using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWalkScarlet : MonoBehaviour
{
    public PlayerMoveCommand m_MoveCommand;
    public Transform m_Scarlet;

    private float m_Speed;

    private void Update()
    {
        m_MoveCommand.TriggerManually(m_Scarlet.forward * m_Speed);
    }

    public void StartWalk()
    {
        m_Speed = 1;
        m_MoveCommand.m_CurrentSpeed = 1;
    }

    public void StartRun()
    {
        m_MoveCommand.m_CurrentSpeed = 3f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWalkScarlet : MonoBehaviour
{
    public PlayerMoveCommand m_MoveCommand;
    public Transform m_Scarlet;

    private float m_Speed;

    public float m_MoveCommandSpeed = 0.7f;

    private void Update()
    {
        m_MoveCommand.TriggerManually(m_Scarlet.forward * m_Speed);
    }

    public void StartWalk()
    {
        m_Speed = 1;
        m_MoveCommand.m_CurrentSpeed = m_MoveCommandSpeed;
    }

    public void StopWalk()
    {
        m_Speed = 0;
        m_MoveCommand.m_CurrentSpeed = 0f;
    }

    public void StartRun()
    {
        m_MoveCommand.m_CurrentSpeed = 3.5f;
        m_Scarlet.GetComponent<Animator>().SetBool("RunAttack", true);
    }

    public void DisableSprintCommand()
    {
        FindObjectOfType<PlayerSprintCommand>().enabled = false;
    }
}

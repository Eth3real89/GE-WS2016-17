using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour, PlayerCommandCallback {

    public PlayerCommand[] m_PlayerCommands;

    void Start () {
        m_PlayerCommands = GetComponentsInChildren<PlayerCommand>();

        foreach(PlayerCommand command in m_PlayerCommands)
        {
            command.Init(this, gameObject, GetComponentInChildren<Animator>());
        }
	}
	
	void Update () {

    }

    public void OnCommandEnd(string commandName, PlayerCommand command)
    {
    }

    public void OnCommandStart(string commandName, PlayerCommand command)
    {
    }
}

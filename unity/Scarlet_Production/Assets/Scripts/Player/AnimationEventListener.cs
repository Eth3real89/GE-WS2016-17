using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Put your listeners for Animation Events in here */

public class AnimationEventListener : MonoBehaviour
{
    public AudioSource m_StepsAudio;
    public PlayerMoveCommand m_MoveCommand;

    private float m_MinimumStepDistance = 0.30f;
    private float m_StepTimer;

    private void Update()
    {
        m_StepTimer -= Time.deltaTime;
    }

    public void StepEvent()
    {
        if (m_StepTimer > 0)
        {
            return;
        }
        m_StepsAudio.volume = 0.2f + (m_MoveCommand.m_CurrentSpeed / 5) * 0.3f;
        m_StepsAudio.Play();
        m_StepTimer = m_MinimumStepDistance;
    }
}

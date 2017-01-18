using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTimer
{
    private float m_StartTime;
    private float m_LerpTime;

    public LerpTimer(float lerpTime = 1)
    {
        m_StartTime = Time.time;
        m_LerpTime = lerpTime;
    }

    public void Start()
    {
        m_StartTime = Time.time;
    }
    
    public void Start(float lerpTime)
    {
        m_LerpTime = lerpTime;
        Start();
    }

    public float GetLerpProgress()
    {
        float currentTime = Time.time;
        float timePassed = Time.time - m_StartTime;
        return Mathf.Min(1f, timePassed / m_LerpTime);
    }
}

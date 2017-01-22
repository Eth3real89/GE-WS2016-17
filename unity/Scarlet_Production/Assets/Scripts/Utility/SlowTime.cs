using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTime : MonoBehaviour {

    private static SlowTime _instance;

    public static SlowTime Instance
    {
        get
        {
            return _instance;
        }
    }

    public float m_SlowAmount;
    
    void Start () {
        _instance = this;
    }
    
    public void StartSlowMo(float slowAmount = -1)
    {
        if (slowAmount < 0)
            slowAmount = m_SlowAmount;

        Time.timeScale = slowAmount;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void StopSlowMo()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}

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
    public bool m_PreventChanges;

    void Start () {
        _instance = this;
        _instance.m_PreventChanges = false;
    }
    
    public void StartSlowMo(float slowAmount = -1)
    {
        if (m_PreventChanges)
            return;

        if (slowAmount < 0)
            slowAmount = m_SlowAmount;

        BossfightJukebox.SetPitch(Mathf.Pow(slowAmount, .17f));
        Time.timeScale = slowAmount;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void StopSlowMo()
    {
        if (m_PreventChanges)
            return;

        BossfightJukebox.SetPitch(1f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}

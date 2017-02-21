using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterController : BossController {


    public BossfightCallbacks m_Callback;

    public void StartPhase(BossfightCallbacks callback)
    {
        this.m_Callback = callback;
        
    }

}

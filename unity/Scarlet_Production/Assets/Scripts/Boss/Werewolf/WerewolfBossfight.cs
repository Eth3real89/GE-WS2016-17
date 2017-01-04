using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfBossfight : MonoBehaviour, BossfightCallbacks {

    public enum Phase {Hunt, Combat, RageMode, Finish};
    public Phase m_StartPhase;

    private Phase m_CurrentPhase;

    public WerewolfHuntController m_HuntController;
    public WerewolfPhase2Controller m_Phase2Controller;
    public WerewolfRagemodeController m_RagemodeController;

	void Start () {
        m_CurrentPhase = m_StartPhase;

        if (m_StartPhase == Phase.Hunt)
        {
            m_HuntController.enabled = true;
            m_HuntController.StartHuntPhase(this);
        }
	}
	
	void Update () {
		
	}

    public void PhaseEnd(BossController whichPhase)
    {
        if (whichPhase == m_HuntController)
        {
            print("Hunt over!");
        }
    }
}

public interface BossfightCallbacks
{
    void PhaseEnd(BossController whichPhase);
}

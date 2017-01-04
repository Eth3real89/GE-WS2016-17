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

    public GameObject m_StreetLightWrapper;

    public GameObject m_Scarlet;

	void Start () {
        m_CurrentPhase = m_StartPhase;

        if (m_StartPhase == Phase.Hunt)
        {
            m_HuntController.enabled = true;
            m_HuntController.StartHuntPhase(this);
        }
        else if (m_StartPhase == Phase.Combat)
        {
            PhaseEnd(m_HuntController);
        }
	}
	
	void Update () {
		
	}

    public void PhaseEnd(BossController whichPhase)
    {
        if (whichPhase == m_HuntController)
        {
            SetStreetLightsEnabled(false);

            m_HuntController.enabled = false;
            m_Phase2Controller.enabled = true;
            m_Phase2Controller.LaunchPhase(this);
        }
        else if (whichPhase == m_Phase2Controller)
        {
            SetStreetLightsEnabled(true);
            if (m_Scarlet != null)
                m_Scarlet.transform.position = new Vector3(0, m_Scarlet.transform.position.y, 0); // @todo better

            m_Phase2Controller.enabled = false;
            m_RagemodeController.enabled = true;
            m_RagemodeController.LaunchPhase(this);
        }
        else if (whichPhase == m_RagemodeController)
        {
            m_RagemodeController.enabled = false;

            print("FIGHT OVER (not really, still need to kill the wolf, but mostly...)");
        }
    }

    private void SetStreetLightsEnabled(bool enabled)
    {
        foreach(Light l in m_StreetLightWrapper.GetComponentsInChildren<Light>())
        {
            l.enabled = enabled;
        }

        foreach(Collider col in m_StreetLightWrapper.GetComponentsInChildren<Collider>())
        {
            if (col.gameObject.name == "Sphere")
            {
                col.enabled = enabled;
            }
        }
    }
}

public interface BossfightCallbacks
{
    void PhaseEnd(BossController whichPhase);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfBossfight : MonoBehaviour {

    public enum Phase {Hunt, Combat, RageMode, Finish};
    public Phase m_StartPhase;

    private Phase m_CurrentPhase;
    
	void Start () {
        m_CurrentPhase = m_StartPhase;
	}
	
	void Update () {
		
	}
}

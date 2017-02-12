using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyBossfight : MonoBehaviour {

    public enum Phase {Phase1, Phase2, Phase3 };

    public Phase m_StartPhase;

    public FairyBossfightPhase m_Phase1;
    public FairyBossfightPhase m_Phase2;
    public FairyBossfightPhase m_Phase3;

    void Start()
    {
        StartCoroutine(StartAfterShortDelay());
    }

    private IEnumerator StartAfterShortDelay()
    {
        yield return new WaitForSeconds(0.2f);

        if (m_StartPhase == Phase.Phase1)
        {
            m_Phase1.enabled = true;
            m_Phase1.StartPhase(this);
        }
        else if (m_StartPhase == Phase.Phase2)
        {
            PhaseEnd(m_Phase1);
        }
        else if (m_StartPhase == Phase.Phase3)
        {
            PhaseEnd(m_Phase2);
        }
    }

    public void PhaseEnd(FairyBossfightPhase whichPhase)
    {
        if (whichPhase == m_Phase1)
        {
            MLog.Log(LogType.BattleLog, "Fairies: Phase 1 over " + this);
            m_Phase1.enabled = false;
            m_Phase2.enabled = true;
            m_Phase2.StartPhase(this);
        }
        else if (whichPhase == m_Phase2)
        {
            MLog.Log(LogType.BattleLog, "Fairies: Phase 2 over " + this);
            m_Phase2.enabled = false;
            m_Phase3.enabled = true;
            m_Phase3.StartPhase(this);
        }
        else if (whichPhase == m_Phase3)
        {
            MLog.Log(LogType.BattleLog, "Fairies: Phase 3 over " + this);
        }
    }
}

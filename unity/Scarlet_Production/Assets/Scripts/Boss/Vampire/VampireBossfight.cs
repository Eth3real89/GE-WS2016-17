using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireBossfight : BossFight, BossfightCallbacks {

    public enum Phase {Tutorial, Phase1, Phase2, Phase3};
    public Phase m_StartPhase;

    public VampirePhase0TutorialController m_TutorialController;
    public VampirePhase1Controller m_Phase1Controller;
    public VampirePhase2Controller m_Phase2Controller;
    public VampirePhase3Controller m_Phase3Controller;

    void Start ()
    {
        StartCoroutine(StartAfterShortDelay());
    }
    private IEnumerator StartAfterShortDelay()
    {
        yield return new WaitForSeconds(0.2f);

        if (m_StartPhase == Phase.Tutorial)
        {
            m_TutorialController.enabled = true;
            m_TutorialController.StartPhase(this);
        }
        else if (m_StartPhase == Phase.Phase1)
        {
            PhaseEnd(m_TutorialController);
        }
        else if (m_StartPhase == Phase.Phase2)
        {
            PhaseEnd(m_Phase1Controller);
        }
        else if (m_StartPhase == Phase.Phase3)
        {
            PhaseEnd(m_Phase2Controller);
        }
    }

    public void PhaseEnd(BossController whichPhase)
    {
        if (whichPhase == m_TutorialController)
        {
            MLog.Log(LogType.BattleLog, "Vampire: Tutorial over " + this);
            m_TutorialController.enabled = false;
            m_Phase1Controller.enabled = true;
            m_Phase1Controller.StartPhase(this);
        }
        else if (whichPhase == m_Phase1Controller)
        {
            MLog.Log(LogType.BattleLog, "Vampire: Phase 1 over " + this);
            m_Phase1Controller.enabled = false;
            m_Phase2Controller.enabled = true;
            m_Phase2Controller.StartPhase(this);
        }
        else if (whichPhase == m_Phase2Controller)
        {
            MLog.Log(LogType.BattleLog, "Vampire: Phase 2 over " + this);
            m_Phase2Controller.enabled = false;
            m_Phase3Controller.enabled = true;
            m_Phase3Controller.StartPhase(this);
        }
        else if (whichPhase == m_Phase3Controller)
        {
            MLog.Log(LogType.BattleLog, "Vampire: Phase 3 over " + this);
            m_Phase3Controller.enabled = false;
            print("Win!");
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelBossfight : BossFight, BossfightCallbacks
{

    public enum Phase { Phase1, Phase2, Phase3 };
    public Phase m_StartPhase;

    public AngelPhase1Controller m_Phase1Controller;
    public AngelPhase2Controller m_Phase2Controller;

    public CharacterHealth m_AngelHealth;

    public GameObject m_DH;
    public GameObject m_DHHealthWrapper;

    void Start()
    {
        StartBossfight();
    }

    public override void StartBossfight()
    {
        base.StartBossfight();
        StartCoroutine(StartAfterShortDelay());
    }

    private IEnumerator StartAfterShortDelay()
    {
        yield return new WaitForSeconds(0.2f);

        m_DH.SetActive(false);
        m_DHHealthWrapper.SetActive(false);

        if (m_StartPhase == Phase.Phase1)
        {
            m_Phase1Controller.enabled = true;
            m_Phase1Controller.StartPhase(this);
        }
        else if (m_StartPhase == Phase.Phase2)
        {
            PhaseEnd(m_Phase1Controller);
        }
    }

    public void PhaseEnd(BossController whichPhase)
    {
        if (whichPhase == m_Phase1Controller)
        {
            MLog.Log(LogType.BattleLog, "Angel: Phase 1 over " + this);

            m_Phase1Controller.enabled = false;
            m_Phase2Controller.enabled = true;
            m_Phase2Controller.StartPhase(this);
        }
        else if (whichPhase == m_Phase2Controller)
        {
            MLog.Log(LogType.BattleLog, "Angel: Phase 2 over " + this);
            m_Phase2Controller.enabled = false;
        }
    }

    protected override void OnScarletDead()
    {
        m_Phase1Controller.CancelAndReset();
        m_Phase2Controller.CancelAndReset();

        m_Phase1Controller.m_NotDeactivated = false;
        m_Phase2Controller.m_NotDeactivated = false;

        base.OnScarletDead();
    }
}

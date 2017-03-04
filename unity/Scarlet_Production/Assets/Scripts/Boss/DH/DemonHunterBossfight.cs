using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterBossfight : BossFight, BossfightCallbacks
{

    public enum Phase { Phase1, Phase2, Phase3 };
    public Phase m_StartPhase;

    public DemonHunterPhase1Controller m_Phase1Controller;
    public DemonHunterPhase2Controller m_Phase2Controller;
    public DemonHunterPhase3Controller m_Phase3Controller;

    public CharacterHealth m_DHHealth;

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

        if (m_StartPhase == Phase.Phase1)
        {
            m_Phase1Controller.enabled = true;
            m_Phase1Controller.StartPhase(this);
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
        if (whichPhase == m_Phase1Controller)
        {
            MLog.Log(LogType.BattleLog, "DH: Phase 1 over " + this);
            m_DHHealth.m_CurrentHealth = m_DHHealth.m_MaxHealth;
            DemonHunterHittable hittable = FindObjectOfType<DemonHunterHittable>();
            if (hittable != null)
                hittable.m_HitCount = 0;
            
            m_Phase1Controller.enabled = false;
            m_Phase2Controller.enabled = true;
            m_Phase2Controller.StartPhase(this);
        }
        else if (whichPhase == m_Phase2Controller)
        {
            MLog.Log(LogType.BattleLog, "DH: Phase 2 over " + this);
            m_Phase2Controller.enabled = false;
            m_Phase3Controller.enabled = true;
            m_Phase3Controller.StartPhase(this);
        }
        else if (whichPhase == m_Phase3Controller)
        {
            MLog.Log(LogType.BattleLog, "DH: Phase 3 over " + this);
            m_Phase3Controller.enabled = false;
        }
    }

}
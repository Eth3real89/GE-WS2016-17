using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        ScarletVOPlayer.Instance.m_Version = ScarletVOPlayer.Version.Church;
        ScarletVOPlayer.Instance.SetupPlayers();

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
            DestroyAllBullets();

            RegenerateScarletAfterPhase();

            DemonHunterHittable hittable = FindObjectOfType<DemonHunterHittable>();
            if (hittable != null)
            {
                hittable.m_HitCount = 0;
                hittable.m_RegenerateHealthOnDeath = false;
            }

            m_Phase1Controller.enabled = false;
            m_Phase2Controller.enabled = true;
            m_Phase2Controller.StartPhase(this);
        }
        else if (whichPhase == m_Phase2Controller)
        {
            DestroyAllBullets();
            MLog.Log(LogType.BattleLog, "DH: Phase 2 over " + this);

            RegenerateScarletAfterPhase();

            m_Phase2Controller.enabled = false;
            m_Phase3Controller.enabled = true;
            m_Phase3Controller.StartPhase(this);
        }
        else if (whichPhase == m_Phase3Controller)
        {
            DestroyAllBullets();

            MLog.Log(LogType.BattleLog, "DH: Phase 3 over " + this);
            m_Phase3Controller.enabled = false;
            ScarletVOPlayer.Instance.PlayVictorySound();
            GetComponent<VictoryScreenController>().ShowVictoryScreen(gameObject);
        }
    }

    protected override void OnScarletDead()
    {
        DemonHunterHittable hittable = FindObjectOfType<DemonHunterHittable>();
        if (hittable != null)
        {
            hittable.m_HitCount = 0;

            hittable.GetComponent<Animator>().SetTrigger("CancelTrigger");
        }

        m_Phase1Controller.CancelAndReset();
        m_Phase2Controller.CancelAndReset();
        m_Phase3Controller.CancelAndReset();

        m_Phase1Controller.enabled = false;
        m_Phase2Controller.enabled = false;
        m_Phase3Controller.enabled = false;

        m_Phase1Controller.m_NotDeactivated = false;
        m_Phase2Controller.m_NotDeactivated = false;
        m_Phase3Controller.m_NotDeactivated = false;

        Gunfire.ResetSound();

        base.OnScarletDead();
        new FARQ().StartTime(115).EndTime(117.6f).Location(m_Phase1Controller.transform).ClipName("dh").Play();
    }

    public override void LoadSceneAfterBossfight()
    {
        SceneManager.LoadScene("pre_angel_scene");
    }
}
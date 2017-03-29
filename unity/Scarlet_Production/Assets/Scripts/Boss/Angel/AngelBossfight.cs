using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AngelBossfight : BossFight, BossfightCallbacks
{

    public enum Phase { Phase1, Phase2 };
    public Phase m_StartPhase;

    public AngelPhase1Controller m_Phase1Controller;
    public AngelPhase2Controller m_Phase2Controller;

    public CharacterHealth m_AngelHealth;
    public GameObject m_AngelRenderer;

    public GameObject m_DH;
    public GameObject m_DHHealthWrapper;

    void Start()
    {
        DigitalRuby.PyroParticles.FireLightScript.s_SkipLightStartup = true;

        StartBossfight();
        StartCoroutine(SaveProgressInPlayerPrefs());
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
        PlayMusic();

        m_DH.SetActive(false);
        m_DHHealthWrapper.SetActive(false);

        if (m_StartPhase == Phase.Phase1)
        {
            m_Phase1Controller.enabled = true;
            m_Phase1Controller.StartPhase(this);
            SetPhaseIndicatorsEnabled(2);
            SetMusicStage(0);
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

            AngelWeapons weapons = FindObjectOfType<AngelWeapons>();
            if (weapons != null)
                weapons.CancelChange();

            RegenerateScarletAfterPhase();

            m_Phase1Controller.CancelAndReset();
            m_Phase1Controller.enabled = false;
            m_Phase2Controller.enabled = true;

            SetPhaseIndicatorsEnabled(1);
            SetMusicStage(1);
            m_Phase2Controller.StartPhase(this);
        }
        else if (whichPhase == m_Phase2Controller)
        {
            DestroyAllBullets();
            m_Phase2Controller.CancelAndReset();

            MLog.Log(LogType.BattleLog, "Angel: Phase 2 over " + this);
            m_Phase2Controller.enabled = false;
            SetPhaseIndicatorsEnabled(0);
            BossfightJukebox.Instance.FadeToVolume(0f);

            PlayerControls pc = FindObjectOfType<PlayerControls>();
            if (pc != null)
            {
                pc.DisableAndLock(pc.m_PlayerCommands);
            }

            try
            {
                m_Phase2Controller.GetComponent<Animator>().SetTrigger("DefeatTrigger");
            } catch { }

            ScarletVOPlayer.Instance.PlayVictorySound();
            PlayScarletVictoryAnimation();
            GetComponent<VictoryScreenController>().ShowVictoryScreen(gameObject);
        }
    }

    private IEnumerator DissolveAngel()
    {
        float t = 0f;
        while ((t += Time.deltaTime) > 1f)
        {
            m_AngelRenderer.GetComponent<Renderer>().material.SetFloat("_Cutoff", t / 1f);
            yield return null;
        }
    }

    protected override void OnScarletDead()
    {
        m_Phase1Controller.CancelAndReset();
        m_Phase2Controller.CancelAndReset();

        m_Phase1Controller.m_NotDeactivated = false;
        m_Phase2Controller.m_NotDeactivated = false;

        m_Phase1Controller.enabled = false;
        m_Phase2Controller.enabled = false;

        base.OnScarletDead();
    }

    public override void LoadSceneAfterBossfight()
    {
        StartCoroutine(LoadSceneDelayed());
    }

    protected IEnumerator LoadSceneDelayed()
    {
        yield return new WaitForSeconds(5f);
        PlayerPrefs.SetString("CurrentLevel", "post_angel_scene");
        FindObjectOfType<ShowSaveSignController>().FadeInSaveSign(LoadScene);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("CurrentLevel"));
    }

    protected override IEnumerator SaveProgressInPlayerPrefs()
    {
        yield return null;
        PlayerPrefs.SetString("CurrentLevel", "dh_angel_battle_dev");
        FindObjectOfType<ShowSaveSignController>().FadeInSaveSign();
        PlayerPrefs.Save();
    }
}

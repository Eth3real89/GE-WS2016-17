using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FairyBossfight : BossFight, FairyPhaseCallbacks {

    public enum Phase {Phase1, Phase2, Phase3, Phase4 };

    public Phase m_StartPhase;

    public FairyBossfightPhase m_Phase1;
    public FairyBossfightPhase m_Phase2;
    public FairyBossfightPhase m_Phase3;
    public FairyBossfightPhase m_Phase4;

    protected Vector3 m_ArmorPositionStart;
    protected Quaternion m_ArmorRotationStart;

    protected Vector3 m_AEPositionStart;
    protected Quaternion m_AERotationStart;

    public GameObject[] m_PhaseIndicatorsArmor;
    public GameObject[] m_PhaseIndicatorsAE;

    void Start()
    {
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
        ArmorFairyHittable armorHittable = FindObjectOfType<ArmorFairyHittable>();
        armorHittable.GetComponent<Animator>().ResetTrigger("ReanimationTrigger");

        ScarletVOPlayer.Instance.m_Version = ScarletVOPlayer.Version.Cave;
        ScarletVOPlayer.Instance.SetupPlayers();
        PlayMusic();

        if (m_StartPhase == Phase.Phase1)
        {
            m_Phase1.enabled = true;
            m_Phase1.StartPhase(this);
            SetPhaseIndicatorsEnabled(3);
            SetMusicStage(0);
        }
        else if (m_StartPhase == Phase.Phase2)
        {
            OnPhaseEnd(m_Phase1);
        }
        else if (m_StartPhase == Phase.Phase3)
        {
            OnPhaseEnd(m_Phase2);
        }
        else if (m_StartPhase == Phase.Phase4)
        {
            OnPhaseEnd(m_Phase3);
            FemaleStartCrying();
        }
    }

    public void OnPhaseEnd(FairyBossfightPhase whichPhase)
    {
        if (whichPhase == m_Phase1)
        {
            MLog.Log(LogType.BattleLog, "Fairies: Phase 1 over " + this);
            DestroyAllBullets();
            m_Phase1.enabled = false;
            SetPhaseIndicatorsEnabled(2);
            SetMusicStage(1);
            m_Phase2.enabled = true;
            m_Phase2.StartPhase(this);
            RegenerateScarletAfterPhase();

            ArmorFairyHittable hittable = FindObjectOfType<ArmorFairyHittable>();
            if (hittable != null)
                hittable.StopPlayingCriticalHPSound();
        }
        else if (whichPhase == m_Phase2)
        {
            MLog.Log(LogType.BattleLog, "Fairies: Phase 2 over " + this);
            DestroyAllBullets();
            m_Phase2.enabled = false;
            SetPhaseIndicatorsEnabled(1);
            SetMusicStage(2);
            m_Phase3.enabled = true;
            m_Phase3.StartPhase(this);
            RegenerateScarletAfterPhase();

            ArmorFairyHittable hittable = FindObjectOfType<ArmorFairyHittable>();
            if (hittable != null)
                hittable.StopPlayingCriticalHPSound();

            ScarletVOPlayer.Instance.PlayVictorySound();
        }
        else if (whichPhase == m_Phase3)
        {
            MLog.Log(LogType.BattleLog, "Fairies: Phase 3 over " + this);
            DestroyAllBullets();
            m_Phase3.enabled = false;
            SetPhaseIndicatorsEnabled(1);
            m_Phase4.enabled = true;
            m_Phase4.StartPhase(this);
        }
        else if (whichPhase == m_Phase4)
        {
            MLog.Log(LogType.BattleLog, "Fairies: Phase 4 over " + this);
            DestroyAllBullets();
            m_Phase4.enabled = false;
            SetPhaseIndicatorsEnabled(0);

            FemaleStopCrying();
            ArmorFairyHittable hittable = FindObjectOfType<ArmorFairyHittable>();
            if (hittable != null)
                hittable.StopPlayingCriticalHPSound();

            GetComponent<VictoryScreenController>().ShowVictoryScreen(gameObject);
            PlayScarletVictoryAnimation();
            ScarletVOPlayer.Instance.PlaySpecialFairyVictorySound();
            PlayerPrefs.SetInt("NumHealthPotions", 4);
        }
    }

    protected override void OnScarletDead()
    {
        bool playTaunt = m_Phase1.enabled || m_Phase2.enabled;

        m_Phase1.m_AEFairyController.gameObject.SetActive(true);
        FemaleStopCrying();

        ArmorFairyHittable hittable = FindObjectOfType<ArmorFairyHittable>();
        if (hittable != null)
        {
            hittable.StopPlayingCriticalHPSound();
            hittable.SetSoundset(true);
        }

        m_Phase1.CancelAndReset();
        m_Phase2.CancelAndReset();
        m_Phase3.CancelAndReset();
        m_Phase4.CancelAndReset();
        m_Phase1.enabled = false;
        m_Phase2.enabled = false;
        m_Phase3.enabled = false;
        m_Phase4.enabled = false;

        base.OnScarletDead();

        if (playTaunt)
        {
            new FARQ().StartTime(85.1f).EndTime(88f).Location(m_Phase1.m_ArmorFairyController.transform).ClipName("armor_fairy").Play();
        }
    }

    protected override void StoreInitialState()
    {
        PlayerHittable playerHittable = FindObjectOfType<PlayerHittable>();
        m_ScarletPositionStart = playerHittable.transform.position + Vector3.zero;
        m_ScarletRotationStart = Quaternion.Euler(playerHittable.transform.rotation.eulerAngles);

        ArmorFairyHittable armorHittable = FindObjectOfType<ArmorFairyHittable>();
        m_ArmorPositionStart = armorHittable.transform.position + Vector3.zero;
        m_ArmorRotationStart = Quaternion.Euler(armorHittable.transform.rotation.eulerAngles);

        AEFairyHittable aeHittable = FindObjectOfType<AEFairyHittable>();
        m_AEPositionStart = aeHittable.transform.position + Vector3.zero;
        m_AERotationStart = Quaternion.Euler(aeHittable.transform.rotation.eulerAngles);
    }

    protected override void ResetInitialPositions()
    {
        PlayerHittable playerHittable = FindObjectOfType<PlayerHittable>();
        playerHittable.transform.position = m_ScarletPositionStart + Vector3.zero;
        playerHittable.transform.rotation = Quaternion.Euler(m_ScarletRotationStart.eulerAngles);

        ArmorFairyHittable armorHittable = FindObjectOfType<ArmorFairyHittable>();
        armorHittable.transform.position = m_ArmorPositionStart + Vector3.zero;
        armorHittable.transform.rotation = Quaternion.Euler(m_ArmorRotationStart.eulerAngles);

        AEFairyHittable aeHittable = FindObjectOfType<AEFairyHittable>();
        if (aeHittable == null)
        {
            ((FairyBossfightPhase3)m_Phase3).m_AEFairy.gameObject.SetActive(true);
            aeHittable = FindObjectOfType<AEFairyHittable>();
        }
        aeHittable.transform.position = m_AEPositionStart + Vector3.zero;
        aeHittable.transform.rotation = Quaternion.Euler(m_AERotationStart.eulerAngles);
        aeHittable.transform.localScale = Vector3.one;
    }

    public override void RestartBossfight()
    {
        AEFairyHittable aeHittable = FindObjectOfType<AEFairyHittable>();
        if (aeHittable == null)
        {
            ((FairyBossfightPhase3)m_Phase3).m_AEFairy.gameObject.SetActive(true);
            aeHittable = FindObjectOfType<AEFairyHittable>();
        }
        CharacterHealth aeHealth = aeHittable.GetComponent<CharacterHealth>();
        aeHealth.m_CurrentHealth = aeHealth.m_HealthStart;

        ArmorFairyHittable armorHittable = FindObjectOfType<ArmorFairyHittable>();
        armorHittable.GetComponent<Animator>().SetBool("Dead", false);
        armorHittable.GetComponent<Animator>().SetTrigger("ReanimationTrigger");
        CharacterHealth armorHealth = armorHittable.GetComponent<CharacterHealth>();
        armorHealth.m_CurrentHealth = armorHealth.m_HealthStart;

        ((FairyBossfightPhase4)m_Phase4).ResetHealthBars();

        base.RestartBossfight();
    }

    public void OnPhaseStart(FairyBossfightPhase phase)
    {
        if (phase is FairyBossfightPhase3)
        {
            FemaleStartCrying();
        }
    }

    public void FemaleStartCrying()
    {
        float start, end;
        if (UnityEngine.Random.value > 0.5)
        {
            start = 31.4f;
            end = 43.6f;
        }
        else
        {
            start = 44.1f;
            end = 50.4f;
        }

        new FARQ().ClipName("ae_fairy").Location(m_Phase3.enabled? m_Phase3.m_AEFairyController.transform : m_Phase4.m_ArmorFairyController.transform)
            .StartTime(start).EndTime(end).Volume(0.5f).OnFinish(FemaleStartCrying).PlayUnlessPlaying();
    }

    public void FemaleStopCrying()
    {
        new FARQ().ClipName("ae_fairy").Location(m_Phase3.m_AEFairyController.transform).StartTime(31.4f).EndTime(43.6f).Volume(0.5f).StopIfPlaying();
        new FARQ().ClipName("ae_fairy").Location(m_Phase3.m_AEFairyController.transform).StartTime(44.1f).EndTime(50.4f).Volume(0.5f).StopIfPlaying();
        new FARQ().ClipName("ae_fairy").Location(m_Phase4.m_ArmorFairyController.transform).StartTime(31.4f).EndTime(43.6f).Volume(0.5f).StopIfPlaying();
        new FARQ().ClipName("ae_fairy").Location(m_Phase4.m_ArmorFairyController.transform).StartTime(44.1f).EndTime(50.4f).Volume(0.5f).StopIfPlaying();
    }

    public override void LoadSceneAfterBossfight()
    {
        PlayerPrefs.SetString("CurrentLevel", "post_fairies_exploration_level");
        PlayerPrefs.Save();
        SceneManager.LoadScene("post_fairies_exploration_level");
    }

    protected override IEnumerator SaveProgressInPlayerPrefs()
    {
        yield return null;
        PlayerPrefs.SetString("CurrentLevel", "fairies_battle_dev");
        PlayerPrefs.Save();
    }

    public override void SetPhaseIndicatorsEnabled(int howMany)
    {
        for (int i = 0; i < m_PhaseIndicatorsArmor.Length; i++)
        {
            if (i < howMany)
            {
                m_PhaseIndicatorsArmor[m_PhaseIndicatorsArmor.Length - i - 1].SetActive(true);
            }
            else
            {
                m_PhaseIndicatorsArmor[m_PhaseIndicatorsArmor.Length - i - 1].SetActive(false);
            }
        }

        for (int i = 0; i < m_PhaseIndicatorsAE.Length; i++)
        {
            if (i < howMany)
            {
                m_PhaseIndicatorsAE[m_PhaseIndicatorsAE.Length - i - 1].SetActive(true);
            }
            else
            {
                m_PhaseIndicatorsAE[m_PhaseIndicatorsAE.Length - i - 1].SetActive(false);
            }
        }
    }
}

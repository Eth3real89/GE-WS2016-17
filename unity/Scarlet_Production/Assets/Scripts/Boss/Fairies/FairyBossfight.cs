using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            m_Phase1.enabled = true;
            m_Phase1.StartPhase(this);
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
            m_Phase1.enabled = false;
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
            m_Phase2.enabled = false;
            m_Phase3.enabled = true;
            m_Phase3.StartPhase(this);
            RegenerateScarletAfterPhase();

            ArmorFairyHittable hittable = FindObjectOfType<ArmorFairyHittable>();
            if (hittable != null)
                hittable.StopPlayingCriticalHPSound();
        }
        else if (whichPhase == m_Phase3)
        {
            MLog.Log(LogType.BattleLog, "Fairies: Phase 3 over " + this);
            m_Phase3.enabled = false;
            m_Phase4.enabled = true;
            m_Phase4.StartPhase(this);
        }
        else if (whichPhase == m_Phase4)
        {
            MLog.Log(LogType.BattleLog, "Fairies: Phase 4 over " + this);
            m_Phase4.enabled = false;

            FemaleStopCrying();
            ArmorFairyHittable hittable = FindObjectOfType<ArmorFairyHittable>();
            if (hittable != null)
                hittable.StopPlayingCriticalHPSound();
        }
    }

    protected override void OnScarletDead()
    {
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
        base.OnScarletDead();
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
        aeHittable.transform.position = m_AEPositionStart + Vector3.zero;
        aeHittable.transform.rotation = Quaternion.Euler(m_AERotationStart.eulerAngles);
    }

    public override void RestartBossfight()
    {
        AEFairyHittable aeHittable = FindObjectOfType<AEFairyHittable>();
        CharacterHealth aeHealth = aeHittable.GetComponent<CharacterHealth>();
        aeHealth.m_CurrentHealth = aeHealth.m_HealthStart;

        ArmorFairyHittable armorHittable = FindObjectOfType<ArmorFairyHittable>();
        CharacterHealth armorHealth = armorHittable.GetComponent<CharacterHealth>();
        armorHealth.m_CurrentHealth = armorHealth.m_HealthStart;

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
}

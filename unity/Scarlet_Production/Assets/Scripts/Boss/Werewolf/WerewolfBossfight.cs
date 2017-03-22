using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WerewolfBossfight : BossFight, BossfightCallbacks {

    public enum Phase {Hunt, Combat, RageMode};

    public float m_TransitionTime = 4f;
    public Phase m_StartPhase;

    public Animator m_WerewolfAnimator;

    public WerewolfHuntController m_HuntController;
    public WerewolfPhase2Controller m_Phase2Controller;
    public WerewolfRagemodeController m_RagemodeController;
    public FadeInRotatingConeAttack m_RagemodeConeAttack;

    public GameObject m_StreetLightWrapper;

    public GameObject m_Scarlet;

    public PlayerControls m_PlayerControls;

    void Start()
    {
        StartBossfight();

        m_RagemodeConeAttack.m_Callback = new EmptyAttackCallback();
    }

    public override void StartBossfight()
    {
        base.StartBossfight();
        StartCoroutine(StartAfterShortDelay());
    }

    private IEnumerator StartAfterShortDelay()
    {
        yield return new WaitForSeconds(0.2f);
        ScarletVOPlayer.Instance.m_Version = ScarletVOPlayer.Version.Forest;
        ScarletVOPlayer.Instance.SetupPlayers();

        if (m_StartPhase == Phase.Hunt)
        {
            StartCoroutine(StartHuntPhaseAfterHowling(0.1f));
        }
        else if (m_StartPhase == Phase.Combat)
        {
            SetRedLightsEnabled(false);
            PhaseEnd(m_HuntController);
        }
        else if (m_StartPhase == Phase.RageMode)
        {
            SetStreetLightsEnabled(false, false);
            SetRedLightsEnabled(true);
            PhaseEnd(m_Phase2Controller);
        }
    }

    public void PhaseEnd(BossController whichPhase)
    {
        if (whichPhase == m_HuntController)
        {
            DestroyAllBullets();
            m_HuntController.enabled = false;
            m_HuntController.m_NotDeactivated = false;

            RegenerateScarletAfterPhase();
            StartCoroutine(FlickerLightsOff());
            StartCoroutine(StartPhase2AfterHowling(2f));
        }
        else if (whichPhase == m_Phase2Controller)
        {
            DestroyAllBullets();
            RegenerateScarletAfterPhase();

            StartCoroutine(FlickerLightsOn(false));
            if (m_Scarlet != null)
                m_Scarlet.transform.position = new Vector3(0, m_Scarlet.transform.position.y, 0); // @todo better
            m_Phase2Controller.enabled = false;
            m_Phase2Controller.m_NotDeactivated = false;

            StartCoroutine(StartPhase3AfterHowling());
        }
        else if (whichPhase == m_RagemodeController)
        {
            DestroyAllBullets();
            m_RagemodeController.enabled = false;
            m_RagemodeController.m_NotDeactivated = false;
            m_RagemodeConeAttack.CancelAttack();
            GetComponent<VictoryScreenController>().ShowVictoryScreen(gameObject);
            PlayScarletVictoryAnimation();
            ScarletVOPlayer.Instance.PlayVictorySound();
        }
    }

    private IEnumerator StartHuntPhaseAfterHowling(float initialWaitTime)
    {
        m_PlayerControls.DisableAllCommands();
        StopPlayerMove();
        yield return new WaitForSeconds(initialWaitTime);

        yield return StartCoroutine(Howl());

        m_PlayerControls.EnableAllCommands();
        m_WerewolfAnimator.SetTrigger("IdleTrigger");
        yield return new WaitForSeconds(0.5f);

        m_HuntController.enabled = true;
        m_HuntController.StartHuntPhase(this);
    }

    private IEnumerator StartPhase2AfterHowling(float initialWaitTime)
    {
        yield return new WaitForSeconds(initialWaitTime);

        m_PlayerControls.DisableAllCommands();
        StopPlayerMove();

        yield return StartCoroutine(Howl());

        m_PlayerControls.EnableAllCommands();
        m_WerewolfAnimator.SetTrigger("IdleTrigger");
        yield return new WaitForSeconds(0.5f);

        m_Phase2Controller.enabled = true;
        m_Phase2Controller.LaunchPhase(this);
    }

    private IEnumerator StartPhase3AfterHowling()
    {
        m_PlayerControls.DisableAllCommands();
        StopPlayerMove();

        m_RagemodeConeAttack.StartAttack();
        yield return StartCoroutine(Phase3IntroHowl());

        SetStreetLightsEnabled(true, false);
        m_PlayerControls.EnableAllCommands();
        m_WerewolfAnimator.SetTrigger("IdleTrigger");
        yield return new WaitForSeconds(0.5f);

        m_RagemodeController.enabled = true;
        m_RagemodeController.LaunchPhase(this);
    }

    private IEnumerator Howl()
    {
        if (FancyAudio.s_UseAudio)
        {
            m_WerewolfAnimator.SetTrigger("HowlTrigger");
            new FARQ().ClipName("werewolf").StartTime(49f).EndTime(56f).Location(m_HuntController.transform).Play();
            yield return new WaitForSeconds(m_TransitionTime);
        }
    }

    private IEnumerator Phase3IntroHowl()
    {
        if (FancyAudio.s_UseAudio)
        {
            new FARQ().ClipName("werewolf").StartTime(179.2f).EndTime(182.5f).Location(m_HuntController.transform).Play();
            yield return new WaitForSeconds(182.5f - 179.2f + 2f);
        }
    }

    private void SetStreetLightsEnabled(bool enabled, bool visualTriggersEnabled)
    {
        foreach(Light l in m_StreetLightWrapper.GetComponentsInChildren<Light>())
        {
            if(l.gameObject.name != "Red Light")
                l.enabled = enabled;
        }

        foreach(Collider col in m_StreetLightWrapper.GetComponentsInChildren<Collider>())
        {
            if (col.gameObject.name == "Sphere")
            {
                col.enabled = enabled;
            }
            else if (col.gameObject.name.Contains("VisualTrigger"))
            {
                col.enabled = visualTriggersEnabled;
            }
        }

        if (!enabled)
        {
            EffectController.Instance.ExitStrongLight();
        }
    }

    private void SetRedLightsEnabled(bool enabled)
    {
        foreach (Light l in m_StreetLightWrapper.GetComponentsInChildren<Light>())
        {
            if (l.gameObject.name == "Red Light")
            {
                l.enabled = enabled;
            }
        }
    }

    private void SetRedLightsIntensity(float intensity)
    {
        foreach (Light l in m_StreetLightWrapper.GetComponentsInChildren<Light>())
        {
            if (l.gameObject.name == "Red Light")
                l.intensity = intensity;
        }
    }

    private IEnumerator FlickerLightsOff()
    {
        for(int i = 0; i < 9; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.02f, 0.08f));
            SetStreetLightsEnabled(i % 2 == 1, i % 2 == 1);
        }

        yield return new WaitForSeconds(1f);

        SetRedLightsEnabled(true);
        float t = 0;
        while ((t += Time.deltaTime) < 2)
        {
            SetRedLightsIntensity(t / 2);
            yield return null;
        }
    }

    protected override void OnScarletDead()
    {
        SetStreetLightsEnabled(true, true);
        SetRedLightsEnabled(false);
        m_HuntController.CancelAndReset();
        m_Phase2Controller.CancelAndReset();
        m_RagemodeController.CancelAndReset();
        m_RagemodeConeAttack.CancelAttack();

        m_HuntController.enabled = false;
        m_Phase2Controller.enabled = false;
        m_RagemodeController.enabled = false;

        OnlyVisualLightField.ResetFields();

        WerewolfHittable hittable = FindObjectOfType<WerewolfHittable>();
        if (hittable != null)
        {
            hittable.GetComponent<Animator>().SetTrigger("IdleTrigger");
            hittable.m_DontPlaySound = false;
            hittable.StopPlayingCriticalHPSound();
        }

        base.OnScarletDead();
    }

    private IEnumerator FlickerLightsOn(bool setEffectsOn)
    {
        SetRedLightsEnabled(false);

        yield return new WaitForSeconds(0.1f);
        SetStreetLightsEnabled(true, setEffectsOn);
        yield return new WaitForSeconds(0.1f);
        SetStreetLightsEnabled(false, false);
        yield return new WaitForSeconds(0.5f);


        for (int i = 0; i < 9; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.02f, 0.08f));
            SetStreetLightsEnabled(i % 2 == 0, i % 2 == 0 && setEffectsOn);
        }
    }

    public override void LoadSceneAfterBossfight()
    {
        SceneManager.LoadScene("post_werewolf_scene");
    }

    private class EmptyAttackCallback : BossAttack.AttackCallback
    {
        public void OnAttackEnd(BossAttack attack)
        {
        }

        public void OnAttackEndUnsuccessfully(BossAttack attack)
        {
        }

        public void OnAttackInterrupted(BossAttack attack)
        {
        }

        public void OnAttackParried(BossAttack attack)
        {
        }

        public void OnAttackStart(BossAttack attack)
        {
        }
    }

}
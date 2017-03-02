using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfBossfight : BossFight, BossfightCallbacks {

    public enum Phase {Hunt, Combat, RageMode, Finish};

    public float m_TransitionTime = 4f;
    public Phase m_StartPhase;

    public Animator m_WerewolfAnimator;

    public WerewolfHuntController m_HuntController;
    public WerewolfPhase2Controller m_Phase2Controller;
    public WerewolfRagemodeController m_RagemodeController;

    public GameObject m_StreetLightWrapper;

    public GameObject m_Scarlet;

    public PlayerControls m_PlayerControls; 

    void Start () {
        StartCoroutine(StartAfterShortDelay());
    }

    private IEnumerator StartAfterShortDelay()
    {
        yield return new WaitForSeconds(0.2f);

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
            SetStreetLightsEnabled(false);
            SetRedLightsEnabled(true);
            PhaseEnd(m_Phase2Controller);
        }
    }

    public void PhaseEnd(BossController whichPhase)
    {
        if (whichPhase == m_HuntController)
        {
            m_HuntController.enabled = false;

            StartCoroutine(FlickerLightsOff());
            StartCoroutine(StartPhase2AfterHowling(2f));
        }
        else if (whichPhase == m_Phase2Controller)
        {
            StartCoroutine(FlickerLightsOn());
            if (m_Scarlet != null)
                m_Scarlet.transform.position = new Vector3(0, m_Scarlet.transform.position.y, 0); // @todo better
            m_Phase2Controller.enabled = false;

            StartCoroutine(StartPhase3AfterHowling());
        }
        else if (whichPhase == m_RagemodeController)
        {
            m_RagemodeController.enabled = false;

            print("FIGHT OVER (not really, still need to kill the wolf, but mostly...)");
        }
    }

    private IEnumerator StartHuntPhaseAfterHowling(float initialWaitTime)
    {
        m_PlayerControls.DisableAllCommands();
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

        yield return StartCoroutine(Howl());

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

    private void SetStreetLightsEnabled(bool enabled)
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
            SetStreetLightsEnabled(i % 2 == 1);
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

    private IEnumerator FlickerLightsOn()
    {
        SetRedLightsEnabled(false);

        yield return new WaitForSeconds(0.1f);
        SetStreetLightsEnabled(true);
        yield return new WaitForSeconds(0.1f);
        SetStreetLightsEnabled(false);
        yield return new WaitForSeconds(0.5f);


        for (int i = 0; i < 9; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.02f, 0.08f));
            SetStreetLightsEnabled(i % 2 == 0);
        }
    }

}
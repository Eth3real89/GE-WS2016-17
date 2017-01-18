using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryCommand : PlayerCommand, HitInterject
{

    public PlayerHittable m_ScarletHittable;

    public ParryCallback m_ParryCallback;

    public float m_TooLateParryTime = 0.05f;
    public float m_PerfectParryTime = 0.025f;
    public float m_OkParryTime = 0.125f;
    public float m_TooSoonParryTime = 0.05f;

    public AudioSource m_ParryAudioPlayer;
    public AudioClip m_BlockAudio;
    public AudioClip m_ParryAudio;

    private enum ParryState { TooLate, Perfect, Ok, TooSoon, None };
    private ParryState m_CurrentState;
    private IEnumerator m_ParryTimer;

    private void Start()
    {
        m_CommandName = "Parry";
        m_CurrentState = ParryState.None;
    }

    public override void InitTrigger()
    {
        m_CommandName = "Parry";
        m_Trigger = new PressAxisTrigger(this, m_CommandName);
    }

    public override void TriggerCommand()
    {
        DoParry();
    }

    private void DoParry()
    {
        m_Callback.OnCommandStart(m_CommandName, this);

        m_ScarletHittable.RegisterInterject(this);
        m_CurrentState = ParryState.TooLate;
        m_ParryTimer = SetParryState(m_TooLateParryTime, ParryState.Perfect);
        StartCoroutine(m_ParryTimer);

        m_Animator.SetTrigger("ParryTrigger");
    }

    private IEnumerator SetParryState(float time, ParryState nextState)
    {
        yield return new WaitForSeconds(time);
        m_CurrentState = nextState;

        if (nextState == ParryState.Perfect)
        {
            m_ParryTimer = SetParryState(m_PerfectParryTime, ParryState.Ok);
            StartCoroutine(m_ParryTimer);
        }
        else if (nextState == ParryState.Ok)
        {
            m_ParryTimer = SetParryState(m_OkParryTime, ParryState.TooSoon);
            StartCoroutine(m_ParryTimer);
        }
        else if (nextState == ParryState.TooSoon)
        {
            m_ParryTimer = SetParryState(m_TooSoonParryTime, ParryState.None);
            StartCoroutine(m_ParryTimer);
        }
        else if (nextState == ParryState.None)
        {
            m_Callback.OnCommandEnd(m_CommandName, this);
        }
    }

    public override void CancelDelay()
    {
        if (m_ParryTimer != null)
            StopCoroutine(m_ParryTimer);
        m_CurrentState = ParryState.None;
    }

    public bool OnHit(Damage dmg)
    {
        if (dmg.Blockable() == Damage.BlockableType.None)
            return false;

        if (m_CurrentState == ParryState.None)
        {
            return false;
        }
        else if (m_CurrentState == ParryState.TooSoon || m_CurrentState == ParryState.TooLate)
        {
            return FailedParry(dmg);
        }
        else if (m_CurrentState == ParryState.Perfect && dmg.Blockable() != Damage.BlockableType.OnlyBlock)
        {
            return PerfectParry(dmg);
        }
        else if (m_CurrentState == ParryState.Ok || (m_CurrentState == ParryState.Perfect && dmg.Blockable() == Damage.BlockableType.OnlyBlock))
        {
            return Block(dmg);
        }

        return false;
    }

    private bool Block(Damage dmg)
    {
        CameraController.Instance.Shake();

        if (m_ParryCallback != null)
            m_ParryCallback.OnBlock();

        CancelDelay();
        m_Callback.OnCommandEnd(m_CommandName, this);

        dmg.OnBlockDamage();
        PlayAudio(m_BlockAudio);

        return true;
    }

    private bool PerfectParry(Damage dmg)
    {
        CameraController.Instance.Shake();

        CancelDelay();
        m_Callback.OnCommandEnd(m_CommandName, this);

        if (m_ParryCallback != null)
            m_ParryCallback.OnPerfectParry();

        dmg.OnParryDamage();
        PlayAudio(m_ParryAudio);

        return true;
    }

    private void PlayAudio(AudioClip clip)
    {
        if (m_ParryAudioPlayer != null)
        {
            m_ParryAudioPlayer.clip = clip;
            m_ParryAudioPlayer.Play();
        }
    }

    private bool FailedParry(Damage dmg)
    {
        CameraController.Instance.Shake();

        if (m_ParryCallback != null)
            m_ParryCallback.OnParryFail();

        dmg.OnSuccessfulHit();
        return true;
    }

    public interface ParryCallback
    {
        void OnPerfectParry();
        void OnBlock();
        void OnParryFail();
    }

}

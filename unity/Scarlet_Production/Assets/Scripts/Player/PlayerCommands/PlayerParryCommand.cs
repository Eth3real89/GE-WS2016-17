﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryCommand : PlayerCommand, HitInterject {

    private Rigidbody m_ScarletBody;
    public PlayerHittable m_ScarletHittable;

    public float m_TooLateParryTime = 0.05f;
    public float m_PerfectParryTime = 0.025f;
    public float m_OkParryTime = 0.125f;
    public float m_TooSoonParryTime = 0.05f;

    private enum ParryState {TooLate, Perfect, Ok, TooSoon, None };
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
        m_Trigger = new DefaultAxisTrigger(this, m_CommandName);
        m_ScarletBody = m_Scarlet.GetComponent<Rigidbody>();
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
        StopCoroutine(m_ParryTimer);
        m_CurrentState = ParryState.None;
    }

    public bool OnHit(Damage dmg)
    {
        if (!dmg.Blockable())
            return false;

        if (m_CurrentState == ParryState.None)
        {
            return false;
        }
        else if (m_CurrentState == ParryState.TooSoon || m_CurrentState == ParryState.TooLate)
        {
            // @todo: Stagger, deal extra damage
            dmg.OnSuccessfulHit();
            return true;
        }
        else if (m_CurrentState == ParryState.Perfect)
        {
            dmg.OnParryDamage();
            return true;
        }
        else if (m_CurrentState == ParryState.Ok)
        {
            dmg.OnBlockDamage();
            return true;
        }

        return false;
    }
}

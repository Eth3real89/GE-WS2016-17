using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitBehaviour : MonoBehaviour, HitInterject {

    public float[] m_TimeWindowsBeforeBlocks;
    public float m_RipostedTimeWindow = 1.5f;

    public Animator m_Animator;

    public float m_StaggerTime = 1f;

    private BossHitCallbacks m_Callbacks;
    private bool m_Active = false;

    private int m_CurrentHit;

    private IEnumerator m_TimeWindowTimer;
    private IEnumerator m_StaggerTimer;

    private bool m_IsStaggered;

    public TurnTowardsScarlet m_TurnCommand;

    private bool m_IsRiposted = false;

    public void Activate(BossHitCallbacks callbacks)
    {
        MLog.Log(LogType.BattleLog, 0, "Activating OnHitBehaviour!");

        m_Animator.SetTrigger("StunTrigger");
        m_CurrentHit = 0;

        m_Active = true;
        m_IsStaggered = false;
        m_IsRiposted = false;
        m_Callbacks = callbacks;

        m_TimeWindowTimer = QuitAfter(m_TimeWindowsBeforeBlocks[m_CurrentHit]);
        StartCoroutine(m_TimeWindowTimer);
    }

    public void ActivateViaRiposte(BossHitCallbacks callbacks)
    {
        m_Animator.SetTrigger("RipostedTrigger");
        m_CurrentHit = 0;

        m_Active = true;
        m_IsStaggered = false;
        m_Callbacks = callbacks;
        m_IsRiposted = true;

        m_TimeWindowTimer = QuitAfter(m_RipostedTimeWindow);
        StartCoroutine(m_TimeWindowTimer);
    }

    public bool OnHit(Damage dmg)
    {
        MLog.Log(LogType.BattleLog, "OnHit, OnHitBehaviour");

        if (!m_Active || m_IsStaggered || m_IsRiposted)
        {
            CameraController.Instance.Shake();
            return false;
        }
        
        if (dmg is BulletDamage)
        {
            dmg.OnSuccessfulHit();
            return false;
        }

        CameraController.Instance.Shake();

        if (m_TurnCommand != null)
            m_TurnCommand.DoTurn();

        if (m_TimeWindowTimer != null)
            StopCoroutine(m_TimeWindowTimer);

        m_CurrentHit++;

        if (m_CurrentHit >= m_TimeWindowsBeforeBlocks.Length)
        {
            TriggerStagger();
        }
        else
        {
            m_Animator.SetTrigger("StunTrigger");
            m_Callbacks.OnBossTakesDamage();
            m_TimeWindowTimer = QuitAfter(m_TimeWindowsBeforeBlocks[m_CurrentHit]);
            StartCoroutine(m_TimeWindowTimer);
        }

        // return false = take damage, so while this _did_ handle the hit, it still needs to return false!
        return false;
    }

    private void TriggerStagger()
    {
        m_IsStaggered = true;
        m_StaggerTimer = EndStaggerAfterWaiting();
        StartCoroutine(m_StaggerTimer);

        m_Animator.SetTrigger("StaggerTrigger");
        m_Callbacks.OnBossStaggered();
    }

    public IEnumerator QuitAfter(float time)
    {
        yield return new WaitForSeconds(time);

        m_Active = false;
        m_Callbacks.OnTimeWindowClosed();
    }

    private IEnumerator EndStaggerAfterWaiting()
    {
        yield return new WaitForSeconds(m_StaggerTime);

        m_Active = false;
        m_Callbacks.OnBossStaggerOver();
    }

    public void CancelBehaviour()
    {
        StopAllCoroutines();
        m_Active = false;
    }

    public interface BossHitCallbacks
    {
        void OnBossTakesDamage();
        void OnBossStaggered();
        void OnBossStaggerOver();

        void OnTimeWindowClosed();
    }
}

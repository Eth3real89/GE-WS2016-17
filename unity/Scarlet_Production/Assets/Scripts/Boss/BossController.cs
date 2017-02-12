using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour, AttackCombo.ComboCallback, BlockingBehaviour.BossBlockCallback, OnHitBehaviour.BossHitCallbacks, HitInterject {

    public GameObject m_Scarlet;

    /// <summary>
    /// 
    /// </summary>
    public AttackCombo[] m_Combos;
    public WerewolfHittable m_BossHittable;

    protected int m_CurrentComboIndex;

    protected IEnumerator m_NextComboTimer;

    protected AttackCombo m_ActiveCombo;

    public BlockingBehaviour m_BlockingBehaviour;
    public int m_MaxBlocksBeforeParry = 3;
    public OnHitBehaviour m_TimeWindowManager;

    // Use this for initialization
    protected void Start ()
    {
/*        RegisterComboCallback();

        m_CurrentComboIndex = 0;

        StartCoroutine(StartAfterDelay()); */
    }

    protected void RegisterComboCallback()
    {
        foreach (AttackCombo combo in m_Combos)
        {
            combo.m_Callback = this;
        }
    }

    protected virtual IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (m_Combos.Length > 0)
        {
            m_Combos[0].LaunchCombo();
        }
    }

    public virtual void OnComboStart(AttackCombo combo)
    {
        MLog.Log(LogType.BattleLog, "On Combo Start, Controller");

        if (m_ActiveCombo != null)
            combo.CancelCombo();
        else
            m_ActiveCombo = combo;
    }

    public virtual void OnComboEnd(AttackCombo combo)
    {
        MLog.Log(LogType.BattleLog, "On Combo End, Controller");

        m_ActiveCombo = null;

        m_BossHittable.RegisterInterject(this);
        m_BlockingBehaviour.m_TimesBlockBeforeParry = m_MaxBlocksBeforeParry;

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        m_NextComboTimer = StartNextComboAfter(combo.m_TimeAfterCombo);
        StartCoroutine(m_NextComboTimer);
    }

    protected virtual IEnumerator StartNextComboAfter(float time)
    {
        yield return new WaitForSeconds(time);
        StartNextCombo();
    }

    protected virtual void StartNextCombo()
    {
        m_CurrentComboIndex++;
        if (m_CurrentComboIndex >= m_Combos.Length)
            m_CurrentComboIndex = 0;

        m_Combos[m_CurrentComboIndex].LaunchCombo();
    }

    public virtual void OnActivateBlock(AttackCombo combo)
    {
        MLog.Log(LogType.BattleLog, "On Activate Block, Controller");

        if (m_ActiveCombo != null)
            m_ActiveCombo.CancelCombo();

        m_BlockingBehaviour.Activate(this);
        m_BossHittable.RegisterInterject(m_BlockingBehaviour);
    }

    public virtual void OnInterruptCombo(AttackCombo combo)
    {
        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        m_NextComboTimer = StartNextComboAfter(combo.m_TimeAfterCombo);
        StartCoroutine(m_NextComboTimer);
    }

    public void OnBossBlocks()
    {
    }

    public void OnBossParries()
    {
        MLog.Log(LogType.BattleLog, "On Boss Parries, Controller");
        StartNextCombo();
    }

    public void OnBlockingOver()
    {
        MLog.Log(LogType.BattleLog, "On Blocking Over, Controller");

        StartNextCombo();
    }

    public virtual bool OnHit(Damage dmg)
    {
        MLog.Log(LogType.BattleLog, "On Hit, Controller");

        if (dmg is BulletDamage)
        {
            dmg.OnSuccessfulHit();
            return false;
        }

        if (m_TimeWindowManager != null)
        {
            m_TimeWindowManager.Activate(this);
            m_BossHittable.RegisterInterject(m_TimeWindowManager);
        }

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        CameraController.Instance.Shake();

        return false;
    }

    public void OnBossTakesDamage()
    {
    }

    public void OnBossStaggered()
    {

    }

    public virtual void OnTimeWindowClosed()
    {
        MLog.Log(LogType.BattleLog, "On Time Window Was Closed, Controller");

        StartNextCombo();
    }

    public virtual void OnBossStaggerOver()
    {
        MLog.Log(LogType.BattleLog, "On Boss Stagger Over, Controller");

        StartNextCombo();
    }


    public virtual void OnComboParried(AttackCombo combo)
    {
    }

    public virtual void OnComboRiposted(AttackCombo combo)
    {
    }

    public virtual void CancelComboIfActive()
    {
        if (m_ActiveCombo != null)
            m_ActiveCombo.CancelCombo();
    }
}

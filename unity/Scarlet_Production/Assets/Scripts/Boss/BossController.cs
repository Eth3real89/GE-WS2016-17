using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour, AttackCombo.ComboCallback, BlockingBehaviour.BossBlockCallback, OnHitBehaviour.BossHitCallbacks, HitInterject {

    public GameObject m_Scarlet;

    public bool m_NotDeactivated = true;

    /// <summary>
    /// 
    /// </summary>
    public AttackCombo[] m_Combos;
    public BossHittable m_BossHittable;

    protected int m_CurrentComboIndex;

    protected IEnumerator m_NextComboTimer;

    protected AttackCombo m_ActiveCombo;

    public BlockingBehaviour m_BlockingBehaviour;
    public int m_MaxBlocksBeforeParry = 3;
    public OnHitBehaviour m_TimeWindowManager;

    protected bool m_AttackActive;
    protected bool m_ComboActive;

    // any angle bigger than that = back attack!
    protected float m_ForwardAngle = 70f;

    protected bool m_OnlyJustStaggered;
    protected IEnumerator m_IFramesAfterStaggerTimer;

    protected void ListenToAttackEvents()
    {
        EventManager.StartListening(BossAttack.ATTACK_START_EVENT, OnAttackStart);
    }

    protected void StopListeningToAttackEvents()
    {

    }

    protected void RegisterComboCallback()
    {
        foreach (AttackCombo combo in m_Combos)
        {
            combo.m_Callback = this;
        }

        ListenToAttackEvents();
    }

    protected virtual IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (m_Combos.Length > 0)
        {
            m_Combos[0].LaunchCombo();
        }

        m_NotDeactivated = true;
    }

    public virtual void OnComboStart(AttackCombo combo)
    {
        MLog.Log(LogType.BattleLog, "On Combo Start, " + this + ", " + combo);

        m_ComboActive = true;

        if (m_ActiveCombo != null)
            combo.CancelCombo();
        else
            m_ActiveCombo = combo;
    }

    public virtual void OnComboEnd(AttackCombo combo)
    {
        MLog.Log(LogType.BattleLog, "On Combo End, Controller");

        m_ActiveCombo = null;
        m_ComboActive = false;

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
        if (!m_NotDeactivated || m_ActiveCombo != null)
            return;

        m_BossHittable.RegisterInterject(this);
        CancelHitBehaviours();

        m_CurrentComboIndex++;
        if (m_CurrentComboIndex >= m_Combos.Length)
            m_CurrentComboIndex = 0;

        m_Combos[m_CurrentComboIndex].LaunchCombo();
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

    public virtual void OnBossParries()
    {
        MLog.Log(LogType.BattleLog, "On Boss Parries, Controller");
        CancelComboIfActive();

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        if (!m_NotDeactivated)
            return;
        StartNextCombo();
    }

    public virtual void OnBlockingOver()
    {
        MLog.Log(LogType.BattleLog, "On Blocking Over, Controller");
        CancelComboIfActive();

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        if (!m_NotDeactivated)
            return;
        StartNextCombo();
    }

    public virtual bool OnHit(Damage dmg)
    {
        MLog.Log(LogType.BattleLog, "On Hit, Controller");
        if (!m_NotDeactivated)
            return true;

        if (dmg is BulletDamage)
        {
            dmg.OnSuccessfulHit();
            return false;
        }

        if (m_ComboActive)
        {
            return HandleHitDuringCombo(dmg);
        }
        else
        {
           return HandleHitOutsideOfCombo(dmg);
        }
    }

    protected virtual bool HandleHitOutsideOfCombo(Damage dmg)
    {
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

    protected virtual bool HandleHitDuringCombo(Damage dmg)
    {
        CancelComboIfActive();

        if (dmg.m_Type == Damage.DamageType.Riposte)
        {
            dmg.OnSuccessfulHit();
            m_TimeWindowManager.ActivateViaRiposte(this);
            m_BossHittable.RegisterInterject(m_TimeWindowManager);

            return false;
        }
        else
        {
            if (IsBackAttack(dmg) && !m_OnlyJustStaggered)
            {
                MLog.Log(LogType.BattleLog, 0, "Back Attack! " + this);

                dmg.OnSuccessfulHit();

                if (m_TimeWindowManager != null)
                {
                    m_TimeWindowManager.Activate(this);
                    m_BossHittable.RegisterInterject(m_TimeWindowManager);
                }
                return false;
            }
            else
            {
                dmg.OnBlockDamage();

                m_BlockingBehaviour.Activate(this);
                m_BossHittable.RegisterInterject(m_BlockingBehaviour);
            }
        }

        return true;
    }

    protected virtual bool IsBackAttack(Damage dmg)
    {
        float angle = BossTurnCommand.CalculateAngleTowards(m_BossHittable.transform, dmg.m_Owner.transform);

        while (angle < -180)
            angle += 360;
        while (angle > 180)
            angle -= 360;

        return Mathf.Abs(angle) >= m_ForwardAngle;
    }

    public virtual void OnBossTakesDamage()
    {
    }

    public virtual void OnBossStaggered()
    {

    }

    public virtual void OnTimeWindowClosed()
    {
        MLog.Log(LogType.BattleLog, "On Time Window Was Closed, Controller");

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);
        CancelComboIfActive();

        if (!m_NotDeactivated)
            return;

        StartNextCombo();
        m_IFramesAfterStaggerTimer = InvulnerableAfterStagger();
        StartCoroutine(m_IFramesAfterStaggerTimer);
    }

    public virtual void OnBossStaggerOver()
    {
        MLog.Log(LogType.BattleLog, "On Boss Stagger Over, Controller");

        m_OnlyJustStaggered = true;

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);
        CancelComboIfActive();

        if (!m_NotDeactivated)
            return;

        StartNextCombo();
        m_IFramesAfterStaggerTimer = InvulnerableAfterStagger();
        StartCoroutine(m_IFramesAfterStaggerTimer);
    }

    protected virtual IEnumerator InvulnerableAfterStagger()
    {
        yield return new WaitForSeconds(0.5f);
        m_OnlyJustStaggered = false;
    }


    public virtual void OnComboParried(AttackCombo combo)
    {
    }

    public virtual void OnComboRiposted(AttackCombo combo)
    {
    }

    public virtual void CancelComboIfActive()
    {
        m_ComboActive = false;

        if (m_ActiveCombo != null)
            m_ActiveCombo.CancelCombo();

        m_ActiveCombo = null;

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);
    }

    protected void OnAttackStart()
    {
        m_AttackActive = true;
    }

    protected void OnAttackEnd()
    {
        m_AttackActive = false;
    }

    protected virtual void ExtremelyDangerousCancelAllCombosEver()
    {
        try
        {
            AttackCombo[] combos = FindObjectsOfType<AttackCombo>();
            foreach(AttackCombo c in combos)
            {
                c.CancelCombo();
            }
        }
        catch { }

        try
        {
            BulletOnExpLaunchCombo[] bulletCombos = FindObjectsOfType<BulletOnExpLaunchCombo>();
            foreach(BulletOnExpLaunchCombo boelc in bulletCombos)
            {
                if (boelc.GetComponentInParent<BulletBehaviour>() != null)
                    boelc.m_AbortOnExpiration = true;
            }
        }
        catch { }
    }

    protected virtual void CancelHitBehaviours()
    {
        if (m_BlockingBehaviour != null)
            m_BlockingBehaviour.CancelBehaviour();

        if (m_TimeWindowManager != null)
            m_TimeWindowManager.CancelBehaviour();
    }

    public virtual void CancelAndReset()
    {
        m_CurrentComboIndex = 0;

        CancelHitBehaviours();
        CancelComboIfActive();
        ExtremelyDangerousCancelAllCombosEver();

        StopAllCoroutines();
    }
}

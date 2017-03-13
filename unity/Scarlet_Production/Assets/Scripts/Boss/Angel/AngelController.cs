using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelController : BossController {

    public AngelWeapons m_Weapons;

    public BossfightCallbacks m_Callback;
    public int m_StartAttackIndex = 0;

    public AngelOnlyMovementCombo[] m_MovementCombos;

    protected AngelComboList m_RegularAttacks;
    protected AngelComboList m_FinishersAndSupers;

    protected AngelComboList m_RangeAttacks;
    protected AngelComboList m_MeleeOrCloseAttacks;
    protected AngelComboList m_CloseGapAttacks;

    protected AngelComboList m_MoveCloseMovements;
    protected AngelComboList m_FeintMovements;
    protected AngelComboList m_StayAwayMovements;

    protected AngelCombo m_ActualLastCombo;

    protected int m_AttackIndex;
    protected bool m_ScarletKnockedDown;

    public int m_DebugAttackIndex = 0;
    public bool m_DebugForceAttack = false;

    protected bool m_InWindup;
    protected bool m_InLongStance;

    public float m_SpeedMultiplier = 1f;

    public virtual void StartPhase(BossfightCallbacks callback)
    {
        //Time.timeScale = 0.5f;
        
        m_ScarletKnockedDown = false;

        SetSpeed(m_SpeedMultiplier);

        MLog.Log(LogType.AngelLog, "Starting Phase, Angel, " + this);
        this.m_Callback = callback;

        RegisterComboCallback();
        m_BossHittable.RegisterInterject(this);

        SetupComboLists();

        StartFirstCombo();
    }

    protected override void RegisterComboCallback()
    {
        base.RegisterComboCallback();

        for(int i = 0; i < m_MovementCombos.Length; i++)
        {
            m_MovementCombos[i].m_Callback = this;
        }
    }

    protected virtual void ListenForWindupEvents()
    {
        EventManager.StartListening(AngelWindupListener.WINDUP_START, WindupStart);
        EventManager.StartListening(AngelWindupListener.WINDUP_END, WindupEnd);
        EventManager.StartListening(AngelWindupListener.LONG_STANCE_START, LongStanceStart);
        EventManager.StartListening(AngelWindupListener.LONG_STANCE_END, LongStanceEnd);
    }

    protected virtual void StopListeningForWindupEvents()
    {
        EventManager.StopListening(AngelWindupListener.WINDUP_START, WindupStart);
        EventManager.StopListening(AngelWindupListener.WINDUP_END, WindupEnd);
        EventManager.StopListening(AngelWindupListener.LONG_STANCE_START, LongStanceStart);
        EventManager.StopListening(AngelWindupListener.LONG_STANCE_END, LongStanceEnd);
    }

    protected virtual void StartFirstCombo()
    {
        if (m_DebugForceAttack)
        {
            foreach(AngelComboList acl in new AngelComboList[] {m_RegularAttacks, m_FinishersAndSupers })
            {
                if (acl.Contains((AngelCombo) m_Combos[m_DebugAttackIndex]))
                {
                    LaunchAngelCombo(acl, acl.ComboIndex((AngelCombo) m_Combos[m_DebugAttackIndex]), -1);
                    break;
                }
            }
        }
        else
        {
            m_CurrentComboIndex = m_StartAttackIndex - 1;
            StartCoroutine(StartNextComboAfter(0.5f));
        }
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        yield return new WaitForSeconds(time);

        DetermineAndLaunchNextAttack();
        //m_Weapons.ChangeTipTo(((AngelCombo)m_Combos[nextCombo]).m_AssociatedTip, OnTipChanged(), this);
    }

    protected virtual void DetermineAndLaunchNextAttack()
    {
        MLog.Log(LogType.AngelLog, "Determine Next Attack " + this);

        if (CheckChainAttacks())
        {
            // CheckChainAttacks already starts the attacks, too
        }
        else if (ScarletIsKnockedDown())
        {
            LaunchSuperOrFinisher();
        }
        else
        {
            m_AttackIndex = m_RegularAttacks.GetRandomCombo();
            AngelOnlyMovementCombo movementCombo = GetMovementCombo(m_AttackIndex);

            if (movementCombo == null || movementCombo.AlreadyInGoodPosition())
            {
                MLog.Log(LogType.AngelLog, "Determine Next Attack: Launching Attack " + this);
                LaunchAngelCombo(m_RegularAttacks, m_AttackIndex);
            }
            else
            {
                MLog.Log(LogType.AngelLog, "Determine Next Attack: Launching Movement " + this);
                movementCombo.LaunchCombo();
            }
        }
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        MLog.Log(LogType.AngelLog, "On Combo End " + combo + " " + this);

        m_ActiveCombo = null;
        m_ComboActive = false;

        m_BossHittable.RegisterInterject(this);
        m_BlockingBehaviour.m_TimesBlockBeforeParry = m_MaxBlocksBeforeParry;

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        if (combo is AngelOnlyMovementCombo)
        {
            OnOnlyMovementComboEnd((AngelOnlyMovementCombo) combo);
        }
        else
        {
            m_ActualLastCombo = (AngelCombo)combo;
            OnAttackComboEnd((AngelCombo) combo);
        }
    }

    protected virtual void OnOnlyMovementComboEnd(AngelOnlyMovementCombo combo)
    {
        MLog.Log(LogType.AngelLog, "On Only Movement Combo End " + combo + " " + this);

        if (combo.m_Success >= 1)
        {
            LaunchAngelCombo(m_RegularAttacks, m_AttackIndex, m_RegularAttacks.ComboAt(m_AttackIndex).m_ActualAttackStartIndex);
        }
        else
        {
            if (combo.m_MovementComboType == AngelOnlyMovementCombo.MovementComboType.MoveAway)
            {
                LaunchMeleeAttack();
            }
            else if (combo.m_MovementComboType == AngelOnlyMovementCombo.MovementComboType.Feint)
            {
                LaunchUniversalPositionAttack();
            }
            else
            {
                LaunchUniversalPositionAttack();
            }
        }
    }

    protected virtual void LaunchMeleeAttack()
    {
        MLog.Log(LogType.AngelLog, "Launching Melee Attack " + this);
        int index = m_MeleeOrCloseAttacks.GetRandomCombo();
        LaunchAngelCombo(m_MeleeOrCloseAttacks, index);
    }

    protected virtual void LaunchUniversalPositionAttack()
    {
        MLog.Log(LogType.AngelLog, "Launching Universal Position Attack " + this);
        int index = m_CloseGapAttacks.GetRandomCombo();
        LaunchAngelCombo(m_CloseGapAttacks, index);
    }

    protected virtual void OnAttackComboEnd(AngelCombo combo)
    {
        MLog.Log(LogType.AngelLog, "On Attack Combo End " + this);

        if (combo.m_Success >= 1)
        {
            m_ScarletKnockedDown = true;
        }
        else
        {
            m_ScarletKnockedDown = false;
        }

        m_NextComboTimer = StartNextComboAfter(combo.m_TimeAfterCombo);
        StartCoroutine(m_NextComboTimer);
    }

    protected virtual AngelOnlyMovementCombo GetMovementCombo(int nextAttack)
    {
        AngelCombo nextUp = m_RegularAttacks.ComboAt(nextAttack);

        if (m_MeleeOrCloseAttacks.Contains(nextUp))
        {
            return (AngelOnlyMovementCombo) m_MoveCloseMovements.ComboAt(m_MoveCloseMovements.GetRandomCombo());
        }
        else if (m_RangeAttacks.Contains(nextUp))
        {
            return (AngelOnlyMovementCombo) m_StayAwayMovements.ComboAt(m_StayAwayMovements.GetRandomCombo());
        }
        else
        {
            return null;
        }
    }

    protected virtual bool CheckChainAttacks()
    {
        if (m_ActualLastCombo != null && m_ActualLastCombo.m_AssociatedTip == AngelWeapons.Tips.Axe &&
            m_ActualLastCombo.m_ComboType == AngelCombo.ComboType.MainRegular)
        {
            if (m_ActualLastCombo.m_Success == 1)
            {
                AngelCombo axeFinisher = FindCombo(AngelCombo.ComboType.MainRegular, AngelWeapons.Tips.Axe);
                if (axeFinisher != null)
                {
                    axeFinisher.LaunchComboFrom(2);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    protected virtual void LaunchSuperOrFinisher()
    {
        MLog.Log(LogType.AngelLog, "Launching Super or Finisher " + this);
        int finisherIndex = m_FinishersAndSupers.GetRandomCombo();
        LaunchAngelCombo(m_FinishersAndSupers, finisherIndex);
    }

    protected virtual void LaunchAngelCombo(AngelComboList list, int comboIndex, int startOverride = -1)
    {
        m_Weapons.ChangeTipTo(list.ComboAt(comboIndex).m_AssociatedTip, OnTipChanged(list, comboIndex, startOverride), this);
    }

    protected virtual bool ScarletIsKnockedDown()
    {
        return m_ActualLastCombo != null && m_ActualLastCombo.m_Success == 1;
    }

    protected virtual IEnumerator OnTipChanged(AngelComboList list, int comboIndex, int startOverride = -1)
    {
        yield return null;
        MLog.Log(LogType.AngelLog, "On Tip Changed, launching: " + list.ComboAt(comboIndex));

        if (startOverride == -1)
        {
            list.ComboAt(comboIndex).LaunchComboFrom(list.StartIndexAt(comboIndex));
        }
        else
        {
            list.ComboAt(comboIndex).LaunchComboFrom(startOverride);
        }
    }

    protected virtual void SetupComboLists()
    {
        m_RegularAttacks = new AngelComboList();
        ReferenceRegularAttacks();

        m_FinishersAndSupers = new AngelComboList();
        ReferenceFinishers();

        m_RangeAttacks = new AngelComboList();
        ReferenceRangeAttacks();

        m_MeleeOrCloseAttacks = new AngelComboList();
        ReferenceMeleeOrCloseAttacks();

        m_CloseGapAttacks = new AngelComboList();
        ReferenceCloseGapAttacks();

        m_MoveCloseMovements = new AngelComboList();
        m_FeintMovements = new AngelComboList();
        m_StayAwayMovements = new AngelComboList();
        ReferenceMovementCombos();
    }

    protected virtual void ReferenceRegularAttacks()
    {
        AddIfFound(AngelCombo.ComboType.MainRegular, AngelWeapons.Tips.Scythe, m_RegularAttacks);
        AddIfFound(AngelCombo.ComboType.MainRegular, AngelWeapons.Tips.Axe, m_RegularAttacks);
        AddIfFound(AngelCombo.ComboType.SideAttack, AngelWeapons.Tips.Spear, m_RegularAttacks);
        AddIfFound(AngelCombo.ComboType.SideAttack, AngelWeapons.Tips.Crosswbow, m_RegularAttacks);
        AddIfFound(AngelCombo.ComboType.SideAttack, AngelWeapons.Tips.Hammer, m_RegularAttacks);
        AddIfFound(AngelCombo.ComboType.SideAttack, AngelWeapons.Tips.Magic, m_RegularAttacks);
    }

    protected virtual void ReferenceFinishers()
    {
        AddIfFound(AngelCombo.ComboType.MainFinisher, AngelWeapons.Tips.Scythe, m_FinishersAndSupers);
        AddIfFound(AngelCombo.ComboType.MainSuper, AngelWeapons.Tips.Scythe, m_FinishersAndSupers);
        AddIfFound(AngelCombo.ComboType.MainFinisher, AngelWeapons.Tips.Axe, m_FinishersAndSupers);
        AddIfFound(AngelCombo.ComboType.MainSuper, AngelWeapons.Tips.Axe, m_FinishersAndSupers);
    }

    protected virtual void ReferenceCloseGapAttacks()
    {
        AddIfFound(AngelCombo.ComboType.MainSuper, AngelWeapons.Tips.Scythe, m_CloseGapAttacks);
        AddIfFound(AngelCombo.ComboType.MainSuper, AngelWeapons.Tips.Axe, m_CloseGapAttacks);
    }

    protected virtual void ReferenceMeleeOrCloseAttacks()
    {
        AddIfFound(AngelCombo.ComboType.MainRegular, AngelWeapons.Tips.Scythe, m_MeleeOrCloseAttacks);
        AddIfFound(AngelCombo.ComboType.MainFinisher, AngelWeapons.Tips.Scythe, m_MeleeOrCloseAttacks);
        AddIfFound(AngelCombo.ComboType.MainRegular, AngelWeapons.Tips.Axe, m_MeleeOrCloseAttacks);
        AddIfFound(AngelCombo.ComboType.MainFinisher, AngelWeapons.Tips.Axe, m_MeleeOrCloseAttacks);
        AddIfFound(AngelCombo.ComboType.SideAttack, AngelWeapons.Tips.Spear, m_MeleeOrCloseAttacks);
        AddIfFound(AngelCombo.ComboType.SideAttack, AngelWeapons.Tips.Hammer, m_MeleeOrCloseAttacks);
    }

    protected virtual void ReferenceRangeAttacks()
    {
        AddIfFound(AngelCombo.ComboType.SideAttack, AngelWeapons.Tips.Crosswbow, m_RangeAttacks);
        AddIfFound(AngelCombo.ComboType.SideAttack, AngelWeapons.Tips.Magic, m_RangeAttacks);
    }

    protected virtual void ReferenceMovementCombos()
    {
        for(int i = 0; i < m_MovementCombos.Length; i++)
        {
            switch(m_MovementCombos[i].m_MovementComboType)
            {
                case AngelOnlyMovementCombo.MovementComboType.MoveAway:
                    m_StayAwayMovements.AddCombo(m_MovementCombos[i]);
                    break;
                case AngelOnlyMovementCombo.MovementComboType.Feint:
                    m_FeintMovements.AddCombo(m_MovementCombos[i]);
                    break;
                case AngelOnlyMovementCombo.MovementComboType.ReachScarlet:
                    m_MoveCloseMovements.AddCombo(m_MovementCombos[i]);
                    break;
            }
        }
    }

    protected virtual void AddIfFound(AngelCombo.ComboType type, AngelWeapons.Tips tip, AngelComboList list, int startIndex = 0)
    {
        AngelCombo combo = FindCombo(type, tip);
        if (combo != null && !list.Contains((AngelCombo)combo))
            list.AddCombo(combo, startIndex);
    }

    protected virtual AngelCombo FindCombo(AngelCombo.ComboType type, AngelWeapons.Tips tip)
    {
        for (int i = 0; i < m_Combos.Length; i++)
        {
            if (((AngelCombo)m_Combos[i]).m_AssociatedTip == tip &&
                ((AngelCombo)m_Combos[i]).m_ComboType == type)
            {
                return (AngelCombo) m_Combos[i];
            }
        }

        return null;
    }

    protected virtual void SetSpeed(float speed)
    {
        Animator animator = GetComponent<Animator>();
        animator.SetFloat("AnimationSpeed", speed);

        AngelAttack.SetSpeedMultiplier(speed);
    }

    public override bool OnHit(Damage dmg)
    {
        if (m_InWindup || m_ActiveCombo == null)
        {
            CancelComboIfActive();

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
        else if (m_InLongStance)
        {
            // @todo: take care of dmg
            return false;
        }
        else
        {
            ReactBasedOnCombo();
            return true;
        }
    }

    protected virtual void ReactBasedOnCombo()
    {
        if (m_ActiveCombo is AngelOnlyMovementCombo)
        {
            // cancel that, dash away, launch ranged attack
        }
        else if (m_FinishersAndSupers.Contains((AngelCombo)m_ActiveCombo))
        {
            // ignore

        } // else maybe individual attacks.... e.g. blocking
        
    }

    protected virtual void WindupStart()
    {
        m_InWindup = true;
    }

    protected virtual void WindupEnd()
    {
        m_InWindup = false;
    }

    protected virtual void LongStanceStart()
    {
        m_InLongStance = true;
    }

    protected virtual void LongStanceEnd()
    {
        m_InLongStance = false;
    }
}

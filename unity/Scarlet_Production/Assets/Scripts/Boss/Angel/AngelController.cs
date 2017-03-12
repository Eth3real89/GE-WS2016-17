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

    protected AngelComboList m_MoveCloseMovments;
    protected AngelComboList m_FeintMovements;
    protected AngelComboList m_StayAwayMovements;

    protected int m_AttackIndex;
    protected bool m_ScarletKnockedDown;

    public virtual void StartPhase(BossfightCallbacks callback)
    {
        //Time.timeScale = 0.5f;
        
        m_ScarletKnockedDown = false;

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

    protected virtual void StartFirstCombo()
    {
        m_CurrentComboIndex = m_StartAttackIndex - 1;
        StartCoroutine(StartNextComboAfter(0.5f));
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
            OnAttackComboEnd((AngelCombo) combo);
        }
    }

    protected virtual void OnOnlyMovementComboEnd(AngelOnlyMovementCombo combo)
    {
        MLog.Log(LogType.AngelLog, "On Only Movement Combo End " + combo + " " + this);

        if (combo.m_Success >= 1)
        {
            LaunchAngelCombo(m_RegularAttacks, m_AttackIndex);
        }
        else
        {
            LaunchUniversalPositionAttack();
        }
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
        return m_MovementCombos[0];
    }

    protected virtual bool CheckChainAttacks()
    {
        return false;
    }

    protected virtual void LaunchSuperOrFinisher()
    {
        MLog.Log(LogType.AngelLog, "Launching Super or Finisher " + this);
        int finisherIndex = m_FinishersAndSupers.GetRandomCombo();
        LaunchAngelCombo(m_FinishersAndSupers, finisherIndex);
    }

    protected virtual void LaunchAngelCombo(AngelComboList list, int comboIndex)
    {
        m_Weapons.ChangeTipTo(list.ComboAt(comboIndex).m_AssociatedTip, OnTipChanged(list, comboIndex), this);
    }

    protected virtual bool ScarletIsKnockedDown()
    {
        return false;
    }

    protected virtual IEnumerator OnTipChanged(AngelComboList list, int comboIndex)
    {
        yield return null;
        MLog.Log(LogType.AngelLog, "On Tip Changed, launching: " + list.ComboAt(comboIndex));
        list.ComboAt(comboIndex).LaunchComboFrom(list.StartIndexAt(comboIndex));
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

        m_MoveCloseMovments = new AngelComboList();
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
                    m_MoveCloseMovments.AddCombo(m_MovementCombos[i]);
                    break;
            }
        }
    }

    protected virtual void AddIfFound(AngelCombo.ComboType type, AngelWeapons.Tips tip, AngelComboList list, int startIndex = 0)
    {
        for (int i = 0; i < m_Combos.Length; i++)
        {
            if (((AngelCombo)m_Combos[i]).m_AssociatedTip == tip &&
                ((AngelCombo)m_Combos[i]).m_ComboType == type &&
                !list.Contains((AngelCombo)m_Combos[i]))
            {
                list.AddCombo((AngelCombo)m_Combos[i], startIndex);
                break;
            }
        }
    }
}

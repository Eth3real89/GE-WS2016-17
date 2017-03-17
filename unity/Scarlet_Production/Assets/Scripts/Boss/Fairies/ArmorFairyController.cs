using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorFairyController : FairyController {

    protected static float[][] s_HeavyAttackSounds =
    {
        new float[] {66.3f, 68f },
    };

    protected static float[][] s_LightAttackSounds =
    {
        new float[] {59.5f, 61.1f },
        new float[] {62.7f, 64.2f },
    };
    private int s_LightAttackSoundCount = -1;

    protected static float[][] s_ParriedAttackSounds =
    {
        new float[] {70.5f, 71.4f },
        new float[] {73.5f, 74.4f },
    };

    protected static float[][] s_StaggerSounds =
    {
        new float[] {76.5f, 77.9f },
        new float[] {79.4f, 80.5f },
    };

    public ArmorFairyParryDamage m_ParryDamage;

    protected FancyAudioRandomClip m_HeavyAttackPlayer;
    protected FancyAudioRandomClip m_LightAttackPlayer;
    protected FancyAudioRandomClip m_ParriedPlayer;
    protected FancyAudioRandomClip m_StaggerPlayer;

    public override void Initialize(FairyControllerCallbacks callbacks)
    {
        base.Initialize(callbacks);
        InitializeAudioPlayers();

        m_NotDeactivated = true;

        RegisterEventsForSound();
    }

    protected virtual void InitializeAudioPlayers()
    {
        m_HeavyAttackPlayer = new FancyAudioRandomClip(s_HeavyAttackSounds, this.transform, "armor_fairy", 1f);
        m_LightAttackPlayer = new FancyAudioRandomClip(s_LightAttackSounds, this.transform, "armor_fairy", 1f);
        m_ParriedPlayer = new FancyAudioRandomClip(s_ParriedAttackSounds, this.transform, "armor_fairy", 1f);
        m_StaggerPlayer = new FancyAudioRandomClip(s_StaggerSounds, this.transform, "armor_fairy", 1f);
    }

    public virtual void RegisterEventsForSound()
    {
        EventManager.StartListening(BossMeleeHitCommand.DOWNSWING_START_EVENT, OnMeleeDownswing);
        EventManager.StartListening(ChargeAttack.START_RUNNING_EVENT, OnChargeStart);
        EventManager.StartListening(BossJumpCommand.JUMP_COMMAND_START, OnJumpStart);
    }

    public virtual void UnRegisterEventsForSound()
    {
        EventManager.StopListening(BossMeleeHitCommand.DOWNSWING_START_EVENT, OnMeleeDownswing);
        EventManager.StopListening(ChargeAttack.START_RUNNING_EVENT, OnChargeStart);
        EventManager.StopListening(BossJumpCommand.JUMP_COMMAND_START, OnJumpStart);
    }

    protected override bool HandleHitOutsideOfCombo(Damage dmg)
    {
        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        CancelComboIfActive();

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

            return true;
        }
    }

    public override void OnBossParries()
    {
        CancelComboIfActive();

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        MLog.Log(LogType.FairyLog, 1, "Boss Parries: Armor " + this);

        Hittable hittable = m_Scarlet.GetComponent<Hittable>();
        if (hittable != null)
        {
            hittable.Hit(m_ParryDamage);
        }

        m_NextComboTimer = StartNextComboAfter(1f);
        StartCoroutine(m_NextComboTimer);
    }

    public void ForceCancelHitBehaviours()
    {
        base.CancelHitBehaviours();
    }


    protected virtual void OnMeleeDownswing()
    {
        PlayLightAttackSound();
    }

    protected virtual void OnChargeStart()
    {
        PlayHeavyAttackSound();
    }

    protected virtual void OnJumpStart()
    {
        PlayHeavyAttackSound();
    }

    public override bool OnHit(Damage dmg)
    {
        bool val = base.OnHit(dmg);

        if (!val)
        {
            CameraController.Instance.Shake();
        }

        return val;
    }

    protected virtual void PlayLightAttackSound(bool definitelyPlay = false)
    {
        if (definitelyPlay || ++s_LightAttackSoundCount % 4 == 0)
            m_LightAttackPlayer.PlayRandomSound();
    }

    protected virtual void PlayHeavyAttackSound()
    {
        m_HeavyAttackPlayer.PlayRandomSound();
    }

    protected virtual void PlayParriedSound()
    {
        m_ParriedPlayer.PlayRandomSound();
    }

    protected virtual void PlayStaggerSound()
    {
        m_StaggerPlayer.PlayRandomSound();
    }

    public override void OnBossStaggered()
    {
        base.OnBossStaggered();
        PlayStaggerSound();
    }

    public override void OnComboParried(AttackCombo combo)
    {
        PlayParriedSound();
        base.OnComboParried(combo);
    }

}

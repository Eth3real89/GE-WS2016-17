using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfController : BossController {

    protected static float[][] s_HeavyAttackSounds =
    {
        new float[] {140.9f, 142.4f },
        new float[] {142.8f, 144.2f },
        new float[] {144.9f, 146.7f },
        new float[] {147.2f, 148.8f },
    };

    protected static float[][] s_LightAttackSounds =
    {
        new float[] {200.5f, 201.2f },
        new float[] {201.4f, 202f },
        new float[] {202.2f, 203.3f },
        new float[] {203.8f, 204.5f },
    };
    private int s_LightAttackSoundCount = -1;

    protected static float[][] s_ParriedAttackSounds =
    {
        new float[] {191.1f, 191.8f },
        new float[] {192.5f, 193.3f },
        new float[] {193.6f, 194.5f },
        new float[] {194.7f, 195.5f },
    };

    protected static float[][] s_StaggerSounds =
    {
        new float[] {167.2f, 170f },
        new float[] {171.1f, 173.5f },
        new float[] {174.6f, 177.6f },
    };

    protected FancyAudioRandomClip m_HeavyAttackPlayer;
    protected FancyAudioRandomClip m_LightAttackPlayer;
    protected FancyAudioRandomClip m_ParriedPlayer;
    protected FancyAudioRandomClip m_StaggerPlayer;

    protected virtual void Start()
    {
        m_HeavyAttackPlayer = new FancyAudioRandomClip(s_HeavyAttackSounds, this.transform, "werewolf", 1f);
        m_LightAttackPlayer = new FancyAudioRandomClip(s_LightAttackSounds, this.transform, "werewolf", 1f);
        m_ParriedPlayer = new FancyAudioRandomClip(s_ParriedAttackSounds, this.transform, "werewolf", 1f);
        m_StaggerPlayer = new FancyAudioRandomClip(s_StaggerSounds, this.transform, "werewolf", 1f);
    }

    protected virtual void RegisterEventsForSound()
    {
        EventManager.StartListening(BossMeleeHitCommand.DOWNSWING_START_EVENT, OnMeleeDownswing);
        EventManager.StartListening(BossJumpCommand.JUMP_COMMAND_START, OnJumpStart);
    }

    protected virtual void UnRegisterEventsForSound()
    {
        EventManager.StopListening(BossMeleeHitCommand.DOWNSWING_START_EVENT, OnMeleeDownswing);
        EventManager.StopListening(BossJumpCommand.JUMP_COMMAND_START, OnJumpStart);
    }

    protected virtual void OnMeleeDownswing()
    {
        // play downswing sound
    }

    protected virtual void OnJumpStart()
    {
        // play jump start sound
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

    public override void OnComboRiposted(AttackCombo combo)
    {
        base.OnComboRiposted(combo);
    }

    public override void OnComboParried(AttackCombo combo)
    {
        PlayParriedSound();
        base.OnComboParried(combo);
    }

}

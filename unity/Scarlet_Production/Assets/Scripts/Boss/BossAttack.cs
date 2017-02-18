using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generally, attacks are part of a combo and consist of general commands and their own
/// specific effects / behaviour.
/// </summary>
public abstract class BossAttack : MonoBehaviour {

    public static string ATTACK_START_EVENT = "boss_attack_start";

    public static AudioClip m_BlockAudio;
    public static AudioSource m_ParryBlockAudioSource;

    public enum PlayerAttackType { None, AnyHit, SpecialHit };

    public PlayerAttackType m_TriggerInterrupt = PlayerAttackType.SpecialHit;
    public PlayerAttackType m_TriggerBlock = PlayerAttackType.None;

    public float m_DamageOnParry = 15f;

    public float m_TimeAfterAttack;

    public Animator m_Animator;
    public GameObject m_Boss;
    public TurnTowardsScarlet m_FullTurnCommand;
    public Hittable m_BossHittable;

    public AttackCallback m_Callback;

    public abstract void CancelAttack();

    public virtual void StartAttack()
    {
        m_Callback.OnAttackStart(this);
        MLog.Log(LogType.BattleLog, 2, "Starting Attack, Attack, " + this);

        EventManager.TriggerEvent(ATTACK_START_EVENT);
    }

    private void PlaySound(AudioClip clip)
    {
        if (m_ParryBlockAudioSource != null)
        {
            m_ParryBlockAudioSource.clip = clip;
            m_ParryBlockAudioSource.Play();
        }
    }

    private bool CheckIfAttackTypeApplies(Damage dmg, PlayerAttackType type)
    {
        if (dmg.m_Type == Damage.DamageType.Riposte) return false;

        if (type == PlayerAttackType.None) return false;
        else if (type == PlayerAttackType.SpecialHit && dmg.m_Type == Damage.DamageType.Special) return true;
        else if (type == PlayerAttackType.AnyHit) return true;

        return false;
    }

    public interface AttackCallback
    {
        /*** notification when this attack starts. */
        void OnAttackStart(BossAttack attack);

        /*** this will only be called if everything went perfectly well & the attack is just over. */
        void OnAttackEnd(BossAttack attack);
        
        /*** e.g. by a timer: When chasing scarlet, the boss never manages to even catch her, so the attack wouldn't end as planned (with an attempted hit). */
        void OnAttackEndUnsuccessfully(BossAttack attack);

        void OnAttackInterrupted(BossAttack attack);

        void OnAttackParried(BossAttack attack);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generally, attacks are part of a combo and consist of general commands and their own
/// specific effects / behaviour.
/// </summary>
public abstract class BossAttack : MonoBehaviour, HitInterject {

    public static AudioClip m_BlockAudio;
    public static AudioSource m_ParryBlockAudioSource;

    public enum PlayerAttackType { None, AnyHit, SpecialHit };

    public PlayerAttackType m_TriggerInterrupt = PlayerAttackType.SpecialHit;
    public PlayerAttackType m_TriggerBlock = PlayerAttackType.None;

    public float m_DamageOnParry = 15f;

    public float m_TimeAfterAttack;

    public Animator m_Animator;
    public GameObject m_Boss;
    public Hittable m_BossHittable;

    public AttackCallback m_Callback;

    public abstract void CancelAttack();

    public virtual void StartAttack()
    {
        MLog.Log(LogType.BattleLog, 2, "Starting Attack, Attack, " + this);

        m_BossHittable.RegisterInterject(this);
        m_Callback.OnAttackStart(this);
    }

    public virtual bool OnHit(Damage dmg)
    {
        MLog.Log(LogType.BattleLog, 2, "On Hit, Attack, " + this);


        if (dmg.m_Type == Damage.DamageType.Riposte)
        {
            m_Callback.OnAttackRiposted(this);
            return false;
        }
        if (CheckBlock(dmg))
        {
            dmg.OnBlockDamage();

            m_Callback.OnBlockPlayerAttack(this);
            PlaySound(m_BlockAudio);

            return true;
        }
        if (CheckInterrupt(dmg))
        {
            m_Callback.OnAttackInterrupted(this);
            return false;
        }

        return false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (m_ParryBlockAudioSource != null)
        {
            m_ParryBlockAudioSource.clip = clip;
            m_ParryBlockAudioSource.Play();
        }
    }

    private bool CheckInterrupt(Damage dmg)
    {
        return CheckIfAttackTypeApplies(dmg, m_TriggerInterrupt);
    }

    private bool CheckBlock(Damage dmg)
    {
        return CheckIfAttackTypeApplies(dmg, m_TriggerBlock);
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

        void OnBlockPlayerAttack(BossAttack attack);

        void OnAttackParried(BossAttack attack);

        void OnAttackRiposted(BossAttack attack);
    }

}

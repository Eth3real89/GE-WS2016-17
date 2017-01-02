using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generally, attacks are part of a combo and consist of general commands and their own
/// specific effects / behaviour.
/// </summary>
public abstract class BossAttack : MonoBehaviour, HitInterject {

    public enum PlayerAttackType { None, AnyHit, AfterBlock, SpecialHit };

    public PlayerAttackType m_TriggerInterrupt = PlayerAttackType.SpecialHit;
    public PlayerAttackType m_TriggerParry = PlayerAttackType.None;
    public PlayerAttackType m_TriggerBlock = PlayerAttackType.None;

    public float m_DamageOnParry = 15f;

    private bool m_LastHitWasBlocked = false;

    public float m_TimeAfterAttack;

    public Animator m_Animator;
    public GameObject m_Boss;
    public Hittable m_BossHittable;

    public AttackCallback m_Callback;
    
	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public abstract void CancelAttack();

    public virtual void StartAttack()
    {
        m_BossHittable.RegisterInterject(this);
        m_LastHitWasBlocked = false;
    }

    public virtual bool OnHit(Damage dmg)
    {
        if (CheckParry(dmg))
        {
            Hittable hittable = dmg.gameObject.GetComponent<Hittable>();
            if (hittable != null)
                hittable.Hit(new BossParryDamage(this));

            dmg.OnParryDamage();
            m_Callback.OnParryPlayerAttack(this);
            return true;
        }
        if (CheckBlock(dmg))
        {
            m_LastHitWasBlocked = true;
            dmg.OnBlockDamage();
            m_Callback.OnBlockPlayerAttack(this);
            return true;
        }
        if (CheckInterrupt(dmg))
        {
            m_Callback.OnAttackInterrupted(this);
            return false;
        }

        m_LastHitWasBlocked = false;
        return false;
    }

    private bool CheckInterrupt(Damage dmg)
    {
        return CheckIfAttackTypeApplies(dmg, m_TriggerInterrupt);
    }

    private bool CheckBlock(Damage dmg)
    {
        return CheckIfAttackTypeApplies(dmg, m_TriggerBlock);
    }

    private bool CheckParry(Damage dmg)
    {
        return CheckIfAttackTypeApplies(dmg, m_TriggerParry);
    }

    private bool CheckIfAttackTypeApplies(Damage dmg, PlayerAttackType type)
    {
        if (type == PlayerAttackType.None) return false;
        else if (type == PlayerAttackType.AfterBlock && m_LastHitWasBlocked) return true;
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

        void OnParryPlayerAttack(BossAttack attack);

        void OnBlockPlayerAttack(BossAttack attack);

        void OnAttackParried(BossAttack attack);
    }

    private class BossParryDamage : Damage
    {
        private BossAttack m_Attack;

        public BossParryDamage(BossAttack attack)
        {
            this.m_Attack = attack;
        }

        public override BlockableType Blockable()
        {
            return BlockableType.None;
        }

        public override float DamageAmount()
        {
            return m_Attack.m_DamageOnParry;
        }
    }

}

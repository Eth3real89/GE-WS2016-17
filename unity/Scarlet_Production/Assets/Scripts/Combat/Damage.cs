using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damage : MonoBehaviour {

    public enum DamageType {Regular, Special};
    public DamageType m_Type = DamageType.Regular;

    public enum BlockableType {None, OnlyBlock, Parry};
    public BlockableType m_BlockType = BlockableType.Parry;

    public DamageCallback m_Callback;

    public bool m_Active;

    public Damage(DamageCallback callback = null)
    {
        m_Callback = callback;
    }

    // these are methods because they will get parameters later! (blockable by which kind of parry etc.)

    public abstract BlockableType Blockable();
    public abstract float DamageAmount();

    /// <summary>
    /// An Attack is blocked = it shouldn't deal damage. The hittable can take care of that, but it
    /// should notify the damage/attack object. 
    /// </summary>
    public virtual void OnBlockDamage()
    {
        if (m_Callback != null)
            m_Callback.OnBlockDamage();
    }

    /// <summary>
    /// While Block might have its own consequences, it just means that no dmg should be dealt.
    /// Parry, however, is a lot stronger: whoever manages to parry should be rewarded (whether it's the boss or scarlet)
    /// </summary>
    public virtual void OnParryDamage()
    {
        if (m_Callback != null)
            m_Callback.OnParryDamage();
    }

    public virtual void OnSuccessfulHit()
    {
        if (m_Callback != null)
            m_Callback.OnSuccessfulHit();
    }

    public interface DamageCallback
    {
        void OnParryDamage();
        void OnBlockDamage();
        void OnSuccessfulHit();
    }

}

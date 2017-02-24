using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHittable : MonoBehaviour, Hittable
{

    public CharacterHealth m_Health;
    protected HitInterject m_Interject;

    public OnHitSignal m_OnHitSignal;

    public AudioSource m_OnHitAudio;
    public AudioSource m_BlockParryAudioSource;

    // @todo it is a weird place for those to be here, but still kind of the best...
    public AudioClip m_BlockAudio;

    public virtual void Hit(Damage damage)
    {
        BossAttack.m_BlockAudio = this.m_BlockAudio;
        BossAttack.m_ParryBlockAudioSource = this.m_BlockParryAudioSource;

        if (m_Interject == null || !m_Interject.OnHit(damage))
        {
            if (damage.DamageAmount() == 0)
                return;

            m_Health.m_CurrentHealth -= damage.DamageAmount();
            damage.OnSuccessfulHit();

            if (m_OnHitSignal != null)
                m_OnHitSignal.OnHit();

            if (m_OnHitAudio != null)
            {
                m_OnHitAudio.Play();
            }
        }
    }

    public virtual void RegisterInterject(HitInterject interject)
    {
        m_Interject = interject;
    }
}

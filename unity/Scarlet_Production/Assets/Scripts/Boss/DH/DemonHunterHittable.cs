using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterHittable : BossHittable { 

    public int m_NumHits = 5;
    public int m_HitCount = 0;

    // @todo it is a weird place for those to be here, but still kind of the best...

    public override void Hit(Damage damage)
    {
        BossAttack.m_BlockAudio = this.m_BlockAudio;
        BossAttack.m_ParryBlockAudioSource = this.m_BlockParryAudioSource;

        if (m_Interject == null || !m_Interject.OnHit(damage))
        {
            if (damage.DamageAmount() == 0)
                return;

            m_HitCount++;

            m_Health.m_CurrentHealth = m_Health.m_MaxHealth * (1f - (m_HitCount / (float) m_NumHits));
            damage.OnSuccessfulHit();

            if (m_OnHitSignal != null)
                m_OnHitSignal.OnHit();

            if (m_OnHitAudio != null)
            {
                m_OnHitAudio.Play();
            }
        }
    }
}

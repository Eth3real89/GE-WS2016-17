using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonHunterHittable : BossHittable {

    protected static float[][] s_HitSounds =
    {
        new float[] {105.1f, 106.4f },
        new float[] {106.7f, 108f },
        new float[] {109.1f, 110.7f },
        new float[] {111.3f, 113f },
    };

    public int m_NumHits = 5;
    public int m_HitCount = 0;
    public bool m_RegenerateHealthOnDeath;

    protected FancyAudioRandomClip m_HitPlayer;

    private void Start()
    {
        m_HitPlayer = new FancyAudioRandomClip(s_HitSounds, this.transform, "dh", 1f);
    }

    public override void Hit(Damage damage)
    {
        BossAttack.m_BlockAudio = this.m_BlockAudio;
        BossAttack.m_ParryBlockAudioSource = this.m_BlockParryAudioSource;

        if (m_Interject == null || !m_Interject.OnHit(damage))
        {
            if (damage.DamageAmount() == 0)
                return;

            m_HitCount++;

            PlayHitSound(damage);

            m_Health.m_CurrentHealth = m_Health.m_MaxHealth * (1f - (m_HitCount / (float) m_NumHits));
            damage.OnSuccessfulHit();

            if (m_HitCount == m_NumHits && m_RegenerateHealthOnDeath)
            {
                StartCoroutine(Regenerate());
            }

            if (m_OnHitSignal != null)
                m_OnHitSignal.OnHit();

            if (m_OnHitAudio != null)
            {
                m_OnHitAudio.Play();
            }
        }
    }

    protected IEnumerator Regenerate()
    {
        float t = 0;
        while ((t += Time.deltaTime) < 1f)
        {
            m_Health.m_CurrentHealth = Mathf.Lerp(0, m_Health.m_MaxHealth, t / 1f);
            yield return null;
        }
        m_Health.m_CurrentHealth = m_Health.m_MaxHealth;
    }

    protected void PlayHitSound(Damage damage)
    {
        m_HitPlayer.PlayRandomSound();
    }
}

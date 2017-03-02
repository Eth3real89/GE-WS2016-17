using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireHittable : BossHittable {

    private int m_LastUsedHitSound = -1;

    public override void Hit(Damage damage)
    {
        float healthBefore = m_Health.m_CurrentHealth;
        base.Hit(damage);

        if (m_Health.m_CurrentHealth < healthBefore)
        {
            PlayHitSound();
        }
    }

    protected void PlayHitSound()
    {
        float[][] sounds = { new float[] {122.1f, 123 },
                             new float[] {124.2f, 125 },
                             new float[] {127, 128 },
                             new float[] {129.3f, 131f },
                             new float[] {134.1f, 135 },
                             new float[] {136.4f, 138f },
                             // @todo maybe there's more */
        };

        int soundIndex;
        do
        {
            soundIndex = UnityEngine.Random.Range(0, sounds.Length);
        } while (soundIndex == m_LastUsedHitSound);

        m_LastUsedHitSound = soundIndex;
       
        float[] sound = sounds[soundIndex];
        new FARQ().ClipName("vampire").Location(transform).StartTime(sound[0]).EndTime(sound[1]).Volume(1).Play();

    }

}

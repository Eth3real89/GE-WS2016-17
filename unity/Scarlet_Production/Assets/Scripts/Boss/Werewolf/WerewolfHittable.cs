using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfHittable : BossHittable {

    private int m_LastUsedHitSound = -1;
    public bool m_DontPlaySound = false;

    public override void Hit(Damage damage)
    {
        float healthBefore = m_Health.m_CurrentHealth;
        base.Hit(damage);

        if (!m_DontPlaySound && m_Health.m_CurrentHealth < healthBefore)
        {
            PlayHitSound();
        }
    }

    protected void PlayHitSound()
    {
        float[][] sounds = { new float[] {191.2f, 192 },
                             new float[] {192.5f, 193.4f },
                             new float[] {193.5f, 194.5f },
                             new float[] {194.6f, 195.5f },
                             new float[] {195.8f, 196.7f },
                             new float[] {197.2f, 198f },
                             new float[] {198.3f, 199f },
                             new float[] {200.5f, 201.1f },
                             new float[] {201.3f, 202f },
                             new float[] {201.3f, 202f },
                             // @todo maybe there's more */
        };

        int soundIndex;
        do
        {
            soundIndex = UnityEngine.Random.Range(0, sounds.Length);
        } while (soundIndex == m_LastUsedHitSound && sounds.Length > 1);

        m_LastUsedHitSound = soundIndex;

        float[] sound = sounds[soundIndex];
        new FARQ().ClipName("werewolf").Location(transform).StartTime(sound[0]).EndTime(sound[1]).Volume(1).Play();

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireHittable : BossHittable {

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

        DecideIfPlayBadlyWoundedSound();
    }

    protected void DecideIfPlayBadlyWoundedSound()
    { // written in a way so it can be re-used if this should be looped?
        if (!m_DontPlaySound && m_Health.m_CurrentHealth < 0.3 * m_Health.m_MaxHealth)
        {
            new FARQ().ClipName("vampire").Location(transform).StartTime(101f).EndTime(117.855f).Volume(1).PlayUnlessPlaying();
        }
    }

    protected void PlayHitSound()
    {
        float[][] sounds = { new float[] {142.1f, 144.1f},
                             new float[] {144.3f, 146.046f},
                             new float[] {146.5f, 148.451f},
                             new float[] {148.6f, 149.942f},
                             new float[] {150.5f, 151.81f},
                             // @todo maybe there's more */
        };

        int soundIndex;
        do
        {
            soundIndex = UnityEngine.Random.Range(0, sounds.Length);
        } while (soundIndex == m_LastUsedHitSound && sounds.Length > 1);

        m_LastUsedHitSound = soundIndex;
       
        float[] sound = sounds[soundIndex];
        new FARQ().ClipName("vampire").Location(transform).StartTime(sound[0]).EndTime(sound[1]).Volume(1).Play();

    }

}

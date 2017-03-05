using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfHittable : BossHittable {
    
    protected static float[][] s_HitSounds =
    {
        new float[] {217.4f, 218.4f },
        new float[] {218.7f, 219.7f },
        new float[] {219.9f, 220.7f },
        new float[] {222.5f, 223.4f },
    };
    private static IEnumerator s_HitSoundTimer;

    protected static float[][] s_RipostedSounds =
    {
        new float[] {156.2f, 158f },
        new float[] {159.7f, 161.4f },
        new float[] {162f, 164f },
    };

    protected FancyAudioRandomClip m_HitPlayer;
    protected FancyAudioRandomClip m_RipostePlayer;

    public bool m_DontPlaySound = false;

    private void Start()
    {
        m_HitPlayer = new FancyAudioRandomClip(s_HitSounds, this.transform, "werewolf", 1f);

        m_RipostePlayer = new FancyAudioRandomClip(s_RipostedSounds, this.transform, "werewolf", 1f);
    }

    public override void Hit(Damage damage)
    {
        float healthBefore = m_Health.m_CurrentHealth;
        base.Hit(damage);

        if (!m_DontPlaySound && m_Health.m_CurrentHealth < healthBefore)
        {
            PlayHitSound(damage);
        }

        if (!m_DontPlaySound && healthBefore >= 0.3f * m_Health.m_MaxHealth && m_Health.m_CurrentHealth < 0.3f * m_Health.m_MaxHealth)
        {
            StartPlayingCriticalHPSound();
        }
    }

    public void StartPlayingCriticalHPSound()
    {
        new FARQ().ClipName("werewolf").Location(this.transform).StartTime(60.5f).EndTime(75.2f).Volume(0.3f).OnFinish(ContinuePlayingCriticalHPSound).PlayUnlessPlaying();
    }

    public void ContinuePlayingCriticalHPSound()
    {
        new FARQ().ClipName("werewolf").Location(this.transform).StartTime(60.5f).EndTime(75.2f).Volume(0.3f).OnFinish(ContinuePlayingCriticalHPSound).Play();
    }

    public void StopPlayingCriticalHPSound()
    {
        new FARQ().ClipName("werewolf").Location(this.transform).StartTime(60.5f).EndTime(75.2f).Volume(0.3f).OnFinish(ContinuePlayingCriticalHPSound).StopIfPlaying();
    }

    protected void PlayHitSound(Damage damage)
    {
        if (damage.m_Type == Damage.DamageType.Riposte)
        {
            m_RipostePlayer.PlayRandomSound();
        }
        else
        {
            if (s_HitSoundTimer != null)
                return;

            m_HitPlayer.PlayRandomSound();
        }

        if (s_HitSoundTimer != null)
            StopCoroutine(s_HitSoundTimer);

        s_HitSoundTimer = HitSoundTimer();
        StartCoroutine(s_HitSoundTimer);
    }

    protected IEnumerator HitSoundTimer()
    {
        yield return new WaitForSeconds(1.3f);
        s_HitSoundTimer = null;
    }
}

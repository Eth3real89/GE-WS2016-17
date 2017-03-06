using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorFairyHittable : BossHittable
{
    protected static float[][] s_HitSounds =
{
        new float[] {40.8f, 42f },
        new float[] {43.1f, 43.9f },
        new float[] {45.5f, 46.8f },
        new float[] {55f, 56.3f },
    };
    private static IEnumerator s_HitSoundTimer;

    protected static float[][] s_RipostedSounds =
    {
        new float[] {70.5f, 71.4f },
        new float[] {73.5f, 74.4f },
    };


    protected FancyAudioRandomClip m_HitPlayer;
    protected FancyAudioRandomClip m_RipostePlayer;

    public bool m_DontPlaySound = false;

    private void Start()
    {
        m_HitPlayer = new FancyAudioRandomClip(s_HitSounds, this.transform, "armor_fairy", 1f);

        m_RipostePlayer = new FancyAudioRandomClip(s_RipostedSounds, this.transform, "armor_fairy", 1f);
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
        new FARQ().ClipName("armor_fairy").Location(this.transform).StartTime(28.9f).EndTime(35.7f).Volume(0.3f).OnFinish(ContinuePlayingCriticalHPSound).PlayUnlessPlaying();
    }

    public void ContinuePlayingCriticalHPSound()
    {
        new FARQ().ClipName("armor_fairy").Location(this.transform).StartTime(28.9f).EndTime(35.7f).Volume(0.3f).OnFinish(ContinuePlayingCriticalHPSound).Play();
    }

    public void StopPlayingCriticalHPSound()
    {
        new FARQ().ClipName("armor_fairy").Location(this.transform).StartTime(28.9f).EndTime(35.7f).Volume(0.3f).OnFinish(ContinuePlayingCriticalHPSound).StopIfPlaying();
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireHittable : BossHittable {

    protected static float[][] s_HitSounds =
    {
        new float[] {142.1f, 144.1f },
        new float[] {144.3f, 146.046f },
        new float[] {146.5f, 148.451f },
        new float[] {148.6f, 149.942f },
        new float[] {150.5f, 151.81f },
    };
    private static IEnumerator s_HitSoundTimer;

    public bool m_DontPlaySound = false;
    protected FancyAudioRandomClip m_HitPlayer;

    private void Start()
    {
        m_HitPlayer = new FancyAudioRandomClip(s_HitSounds, this.transform, "vampire", 1f);
    }

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
            StartPlayingCriticalHPSound();
        }
    }

    public void StartPlayingCriticalHPSound()
    {
        new FARQ().ClipName("vampire").Location(transform).StartTime(101f).EndTime(117.855f).Volume(0.3f).OnFinish(ContinuePlayingCriticalHPSound).PlayUnlessPlaying();
    }

    public void ContinuePlayingCriticalHPSound()
    {
        new FARQ().ClipName("vampire").Location(transform).StartTime(101f).EndTime(117.855f).Volume(0.3f).OnFinish(ContinuePlayingCriticalHPSound).Play();
    }

    public void StopPlayingCriticalHPSound()
    {
        new FARQ().ClipName("vampire").Location(transform).StartTime(101f).EndTime(117.855f).Volume(0.3f).OnFinish(ContinuePlayingCriticalHPSound).StopIfPlaying();
    }

    protected void PlayHitSound()
    {
        if (s_HitSoundTimer != null)
            return;

        m_HitPlayer.PlayRandomSound();
        
        s_HitSoundTimer = HitSoundTimer();
        StartCoroutine(s_HitSoundTimer);
    }

    protected IEnumerator HitSoundTimer()
    {
        yield return new WaitForSeconds(1.3f);
        s_HitSoundTimer = null;
    }
}

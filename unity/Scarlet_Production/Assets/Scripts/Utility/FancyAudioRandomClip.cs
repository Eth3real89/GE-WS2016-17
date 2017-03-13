using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyAudioRandomClip {

    protected int m_LastUsedSoundIndex;
    protected float[][] m_Sounds;

    protected Transform m_Transform;
    protected string m_AudioName;
    protected float m_Volume;

    public FancyAudioRandomClip(float[][] sounds, Transform transform, string audioName, float volume = 1f)
    {
        m_Sounds = sounds;
        m_LastUsedSoundIndex = -1;
        m_Transform = transform;
        m_AudioName = audioName;
        m_Volume = volume;
    }

    public void PlayRandomSound(bool stopLast = false)
    {
        if (stopLast && m_LastUsedSoundIndex > -1)
            new FARQ().ClipName(m_AudioName).Location(m_Transform).StartTime(m_Sounds[m_LastUsedSoundIndex][0]).EndTime(m_Sounds[m_LastUsedSoundIndex][1]).Volume(m_Volume).StopIfPlaying();

        int soundIndex = ChooseAudioClipData();
        float[] sound = m_Sounds[soundIndex];

        new FARQ().ClipName(m_AudioName).Location(m_Transform).StartTime(sound[0]).EndTime(sound[1]).Volume(m_Volume).Play();
    }

    protected int ChooseAudioClipData()
    {
        int soundIndex;
        do
        {
            soundIndex = Random.Range(0, m_Sounds.Length);
        } while (soundIndex == m_LastUsedSoundIndex && m_Sounds.Length > 1);

        return soundIndex;
    }

}

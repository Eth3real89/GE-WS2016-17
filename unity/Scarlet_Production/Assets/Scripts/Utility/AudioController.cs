using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioController : GenericSingletonClass<AudioController>
{
    public SoundFile[] m_SoundFiles;
    private Dictionary<string, AudioSource> m_AudioDict;

    public delegate void FadeAudioCallback();

    [Serializable]
    public struct SoundFile
    {
        public string name;
        public AudioClip file;
        public bool loop;
    }

    private void Start()
    {
        m_AudioDict = new Dictionary<string, AudioSource>();
        CreateAudioDictionary();
        PlaySound("Atmosphere", 0.6f);
    }

    private void CreateAudioDictionary()
    {
        foreach (SoundFile file in m_SoundFiles)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>() as AudioSource;
            source.loop = file.loop;
            source.clip = file.file;
            m_AudioDict.Add(file.name, source);
        }
    }

    public void PlaySound(string name, float volume = 1)
    {
        AudioSource source = GetAudioSource(name);
        if (source == null)
            return;

        source.volume = volume;
        source.Play();
    }

    public void StopSound(string name)
    {
        AudioSource source = GetAudioSource(name);
        if (source == null)
            return;

        source.Stop();
    }

    public void AdjustVolume(string name, float volume)
    {
        AudioSource source = GetAudioSource(name);
        if (source == null)
            return;

        source.volume = volume;
    }

    public void AdjustAllVolumes(float volume, params string[] ignoreSounds)
    {
        List<string> ignoreList = new List<string>(ignoreSounds);
        foreach (string key in m_AudioDict.Keys)
        {
            if (!ignoreList.Contains(key))
                m_AudioDict[key].volume = volume;
        }
    }

    public void FadeIn(string name, float volume, float time, FadeAudioCallback callback = null)
    {
        AudioSource source = GetAudioSource(name);
        if (source == null)
            return;

        source.volume = 0;
        source.Play();
        StartCoroutine(FadeVolume(source, 0, volume, time, callback));
    }

    public void FadeOut(string name, float time, FadeAudioCallback callback = null)
    {
        AudioSource source = GetAudioSource(name);
        if (source == null)
            return;

        StartCoroutine(FadeVolume(source, source.volume, 0, time, callback));
    }

    public void FadeTo(string name, float time, float volume, FadeAudioCallback callback = null)
    {
        AudioSource source = GetAudioSource(name);
        if (source == null)
            return;

        StartCoroutine(FadeVolume(source, source.volume, volume, time, callback));
    }

    IEnumerator FadeVolume(AudioSource source, float volStart, float volEnd, float time, FadeAudioCallback callback)
    {
        source.volume = volStart;
        LerpTimer timer = new LerpTimer(time);
        timer.Start();
        while (timer.GetLerpProgress() < 1)
        {
            source.volume = Mathf.Lerp(volStart, volEnd, timer.GetLerpProgress());
            yield return null;
        }
        if (callback != null)
            callback();
    }

    private AudioSource GetAudioSource(string name)
    {
        if (!m_AudioDict.ContainsKey(name))
        {
            Debug.LogError("Audio with Key " + name + " doesn't exist in Dictionary!");
            return null;
        }
        return m_AudioDict[name];
    }
}

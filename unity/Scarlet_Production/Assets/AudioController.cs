using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioController : GenericSingletonClass<AudioController>
{
    public SoundFile[] m_SoundFiles;
    private Dictionary<string, AudioSource> m_AudioDict;

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
        if (!m_AudioDict.ContainsKey(name))
        {
            Debug.LogError("Audio with Key " + name + " doesn't exist in Dictionary!");
            return;
        }
        AudioSource source = m_AudioDict[name];
        source.volume = volume;
        source.Play();
    }

    public void StopSound(string name)
    {
        if (!m_AudioDict.ContainsKey(name))
        {
            Debug.LogError("Audio with Key " + name + " doesn't exist in Dictionary!");
            return;
        }
        AudioSource source = m_AudioDict[name];
        source.Stop();
    }
}

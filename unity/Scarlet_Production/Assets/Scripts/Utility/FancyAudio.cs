using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyAudio : MonoBehaviour {

    public static bool s_UseAudio = true;

    private static FancyAudio _Instance;

    public static FancyAudio Instance
    {
        get
        {
            return _Instance;
        }
    }

    public AudioSource m_SourcePrefab;

    protected Dictionary<Transform, List<AudioSource>> m_Sources;
    protected Dictionary<string, AudioClip> m_Clips;
    
	void Start () {
		if (_Instance == null)
        {
            _Instance = this;
            _Instance.m_Clips = new Dictionary<string, AudioClip>();
            _Instance.m_Sources = new Dictionary<Transform, List<AudioSource>>();
        }
	}

    public void RegisterClip(string s, AudioClip clip)
    {
        _Instance.m_Clips.Add(s, clip);
    }

    public void Play(FARQ rq)
    {
        if (!_Instance.m_Clips.ContainsKey(rq.m_ClipName))
            return;

        AudioSource source = FindAudioSource(rq.m_Position);

        source.clip = _Instance.m_Clips[rq.m_ClipName];
        source.loop = rq.m_Loop;
        source.time = rq.m_Start;
        source.volume = rq.m_Volume;
        source.Play();

        float duration;
        if (rq.m_End == -1)
        {
            duration = source.clip.length - rq.m_Start;
        }
        else
        {
            duration = rq.m_End - rq.m_Start;
        }

        _Instance.StartCoroutine(StopPlaying(source, duration));
    }

    private IEnumerator StopPlaying(AudioSource source, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        source.Stop();
    }

    private AudioSource FindAudioSource(Transform m_Position)
    {
        List<AudioSource> sources = null;
        if (_Instance.m_Sources.ContainsKey(m_Position))
        {
            sources = _Instance.m_Sources[m_Position];
        }

        if (sources == null)
        {
            sources = new List<AudioSource>();
            _Instance.m_Sources.Add(m_Position, sources);
        }

        AudioSource emptySource = null;
        foreach (AudioSource source in sources)
        {
            if (!source.isPlaying)
            {
                emptySource = source;
                break;
            }
        }
        
        if (emptySource == null)
        {
            emptySource = GameObject.Instantiate(m_SourcePrefab, m_Position);
            sources.Add(emptySource);
        }

        return emptySource;
    }
}

/// <summary>
/// "Fancy Audio ReQuest"
/// </summary>
public class FARQ
{
    public Transform m_Position;

    public string m_ClipName;

    public float m_Start;
    public float m_End;

    public float m_Volume;

    public bool m_Loop;

    public FARQ()
    {
        // default values:
        m_Loop = false;
        m_End = -1f;
        m_Start = 0f;
        m_Volume = 1f;
    }

    public FARQ Location(string findLocation)
    {
        // @todo
        return this;
    }

    public FARQ Location(Transform location)
    {
        m_Position = location;
        return this;
    }

    public FARQ ClipName(string name)
    {
        m_ClipName = name;
        return this;
    }

    public FARQ StartTime(float seconds)
    {
        m_Start = seconds;
        return this;
    }

    public FARQ EndTime(float seconds)
    {
        m_End = seconds;
        return this;
    }

    public FARQ Volume(float volume)
    {
        m_Volume = volume;
        return this;
    }

    public FARQ Loop(bool loop)
    {
        m_Loop = loop;
        return this;
    }

    public void Play()
    {
        FancyAudio.Instance.Play(this);
    }

}

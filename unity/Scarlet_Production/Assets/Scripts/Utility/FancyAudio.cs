 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class FancyAudio : MonoBehaviour {

    public static bool s_UseAudio = true;

    private static float _s_MasterVolume = 0.7f;
    public static float s_MasterVolume
    {
        set
        {
            SetMasterVolume(value);
        }
    }

    private static void SetMasterVolume(float value)
    {
        _s_MasterVolume = value; 
        if (_Instance == null)
        {
        }
            
        foreach(Transform t in _Instance.m_Sources.Keys)
        {
            foreach(SourceAndRequest sar in _Instance.m_Sources[t])
            {
                sar.m_Source.volume = sar.m_Request.m_Volume * _s_MasterVolume;
            }
        }
    }

    private static FancyAudio _Instance;

    public static FancyAudio Instance
    {
        get
        {
            return _Instance;
        }
    }

    public AudioSource m_SourcePrefab;

    protected Dictionary<Transform, List<SourceAndRequest>> m_Sources;
    protected Dictionary<string, AudioClip> m_Clips;
    
	void Start () {
		if (_Instance == null)
        {
            _Instance = this;
            _Instance.m_Clips = new Dictionary<string, AudioClip>();
            _Instance.m_Sources = new Dictionary<Transform, List<SourceAndRequest>>();
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

        AudioSource source = FindAudioSource(rq);

        source.clip = _Instance.m_Clips[rq.m_ClipName];

        source.loop = rq.m_Loop;
        source.time = rq.m_Start;
        source.volume = rq.m_Volume * _s_MasterVolume;
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

        _Instance.StartCoroutine(StopPlaying(source, duration, rq));
    }

    private IEnumerator StopPlaying(AudioSource source, float seconds, FARQ originalRequest)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (source != null && source.isPlaying)
        {
            if (!originalRequest.m_Loop)
                source.Stop();

            if (originalRequest.m_OnFinish != null)
                originalRequest.m_OnFinish();
        }
    }

    public List<AudioSource> SearchByParams(FARQ rq)
    {
        List<AudioSource> resultSources = new List<AudioSource>();

        Transform position = rq.m_Position;

        if (_Instance.m_Sources.ContainsKey(position))
        {
            List<SourceAndRequest> sourcesOnTarget = _Instance.m_Sources[position];
            for(int i = 0; i < sourcesOnTarget.Count; i++)
            {
                if (FARQ.Compare(rq, sourcesOnTarget[i].m_Request)) {
                    resultSources.Add(sourcesOnTarget[i].m_Source);
                }
            }
        }

        return resultSources;
    }

    /// <summary>
    /// Finds an empty Audio Source among the children of m_Position.
    /// Will create one if there is no idle audio source.
    /// (Does not affect sources that were not created by FancyAudio.)
    /// </summary>
    /// <param name="m_Position"></param>
    private AudioSource FindAudioSource(FARQ request)
    {
        Transform position = request.m_Position;

        List<SourceAndRequest> sources = null;
        if (_Instance.m_Sources.ContainsKey(position))
        {
            sources = _Instance.m_Sources[position];
        }

        if (sources == null)
        {
            sources = new List<SourceAndRequest>();
            _Instance.m_Sources.Add(position, sources);
        }

        AudioSource emptySource = null;
        foreach (SourceAndRequest source in sources)
        {
            if (!source.m_Source.isPlaying)
            {
                emptySource = source.m_Source;
                source.m_Request = request;
                break;
            }
        }
        
        if (emptySource == null)
        {
            emptySource = GameObject.Instantiate(m_SourcePrefab, position);

            SourceAndRequest sar = new SourceAndRequest();
            sar.m_Request = request;
            sar.m_Source = emptySource;

            sources.Add(sar);
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

    public UnityAction m_OnFinish;

    public FARQ()
    {
        // default values:
        m_Loop = false;
        m_End = -1f;
        m_Start = 0f;
        m_Volume = 1f;
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

    /// <summary>
    /// Does <em>NOT</em> loop the selected part of the file!!
    /// Sets the "loop" param on the Audio Source instead.
    /// </summary>
    /// <param name="loop"></param>
    /// <returns></returns>
    public FARQ Loop(bool loop)
    {
        m_Loop = loop;
        return this;
    }
    
    public FARQ OnFinish(UnityAction onFinish)
    {
        m_OnFinish = onFinish;
        return this;
    }

    public void Play()
    {
        FancyAudio.Instance.Play(this);
    }

    public void PlayUnlessPlaying()
    {
        List<AudioSource> sources = FancyAudio.Instance.SearchByParams(this);

        bool playing = false;
        for(int i = 0; i < sources.Count; i++)
        {
            if (sources[i] != null && sources[i].isPlaying)
            {
                playing = true;
            }
        }

        if (!playing)
            this.Play();
    }

    public void StopIfPlaying()
    {
        if (FancyAudio.Instance == null)
            return;

        List<AudioSource> sources = FancyAudio.Instance.SearchByParams(this);

        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i] != null && sources[i].isPlaying)
            {
                sources[i].Stop();
            }
        }
    }

    public static bool Compare(FARQ f1, FARQ f2)
    {
        return f1.m_ClipName == f2.m_ClipName &&
            f1.m_Position == f2.m_Position &&
            f1.m_Start == f2.m_Start;
    }
}

public class SourceAndRequest
{
    public AudioSource m_Source;
    public FARQ m_Request;
}

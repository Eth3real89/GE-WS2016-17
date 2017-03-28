using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossfightJukebox : MonoBehaviour {

    protected static BossfightJukebox _Instance;

    public static BossfightJukebox Instance
    {
        get
        {
            return _Instance;
        }
    }

    public static void SetVolume(float volume)
    {
        if (_Instance != null)
        {
            _Instance.m_Source1.volume = volume;
            _Instance.m_Source2.volume = volume;
        }
    }

    public static void SetPitch(float pitch)
    {
        if (_Instance != null)
        {
            _Instance.m_Source1.pitch = pitch;
            _Instance.m_Source2.pitch = pitch;
        }
    }

    public AudioClip[] m_Clips;

    public AudioSource m_Source1;
    public AudioSource m_Source2;

    public int m_CurrentClipOffset = 0;

    public float m_FadeSpeed = 3f;

    protected int m_PlayingClip;
    protected IEnumerator m_ClipEnumerator;

    private void Start()
    {
        if (_Instance == null)
            _Instance = this;
    }

    public void StartPlaying()
    {
        m_PlayingClip = m_CurrentClipOffset;
        m_Source1.clip = m_Clips[m_PlayingClip];
        m_Source1.Play();

        m_ClipEnumerator = WhilePlaying();
        StartCoroutine(m_ClipEnumerator);
    } 

    public void StartPlayingUnlessPlaying()
    {
        if (m_ClipEnumerator == null)
            StartPlaying();
    }

    protected IEnumerator WhilePlaying()
    {
        bool s1Active = m_Source1.isPlaying;

        AudioSource activeSource = s1Active ? m_Source1 : m_Source2;
        AudioSource nextSource = s1Active ? m_Source2 : m_Source1;
        while (activeSource.isPlaying)
        {
            yield return null;
        }
        activeSource.Stop();

        m_PlayingClip = m_CurrentClipOffset;

        nextSource.clip = m_Clips[m_PlayingClip];
        nextSource.Play();

        m_ClipEnumerator = WhilePlaying();
        StartCoroutine(m_ClipEnumerator);
    }

    public void FadeToVolume(float volume)
    {
        StartCoroutine(FadeVolumeRoutine(volume));
    }

    protected IEnumerator FadeVolumeRoutine(float volumeToReach)
    {
        float t = 0;
        while((t += Time.deltaTime) < m_FadeSpeed)
        {
            SetVolume(t / m_FadeSpeed * volumeToReach);
            yield return null;
        }

        SetVolume(volumeToReach);
    }

    public void StopPlaying()
    {
        if (m_ClipEnumerator != null)
        {
            StopCoroutine(m_ClipEnumerator);
            m_ClipEnumerator = null;
        }

        m_Source1.Stop();
        m_Source2.Stop();
    }
	
}

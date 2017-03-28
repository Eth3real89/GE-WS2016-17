using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsHandler : MonoBehaviour
{
    public enum StepType
    {
        Brick, Grass, Wood, BrickReverb
    }

    [System.Serializable]
    public struct StepSound
    {
        public StepType m_Type;
        public AudioClip m_Sound;
    }

    public List<StepSound> m_StepSounds;

    private AudioSource m_Source;
    private Dictionary<StepType, AudioClip> m_StepMapping;

    private void Start()
    {
        m_Source = GetComponent<AudioSource>();
        m_StepMapping = new Dictionary<StepType, AudioClip>();

        foreach (StepSound s in m_StepSounds)
        {
            m_StepMapping.Add(s.m_Type, s.m_Sound);
        }
    }

    public void SetStepSound(StepType type)
    {
        m_Source.clip = m_StepMapping[type];
    }

    public void OnHitGround()
    {
        m_Source.volume = 1;
        m_Source.Play();
    }

    public void OnLoseGround()
    {
        m_Source.volume = 0;
    }
}

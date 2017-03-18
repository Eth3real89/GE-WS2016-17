﻿using UnityEngine;
using System.Collections;

namespace DigitalRuby.PyroParticles
{
    /// <summary>
    /// Provides an easy wrapper to looping audio sources with nice transitions for volume when starting and stopping
    /// </summary>
    public class LoopingAudioSource
    {
        public AudioSource AudioSource { get; private set; }
        public float TargetVolume { get; set; }

        private float startMultiplier;
        private float stopMultiplier;
        private float currentMultiplier;

        public LoopingAudioSource(MonoBehaviour script, AudioSource audioSource, float startMultiplier, float stopMultiplier)
        {
            AudioSource = audioSource;
            if (audioSource != null)
            {
                AudioSource.loop = true;
                AudioSource.volume = 0.0f;
                AudioSource.Stop();
            }

            TargetVolume = 1.0f;

            this.startMultiplier = currentMultiplier = startMultiplier;
            this.stopMultiplier = stopMultiplier;
        }

        public void Play()
        {
            Play(TargetVolume);
        }

        public void Play(float targetVolume)
        {
            if (AudioSource != null && !AudioSource.isPlaying)
            {
                AudioSource.volume = 0.0f;
                AudioSource.Play();
                currentMultiplier = startMultiplier;
            }
            TargetVolume = targetVolume;
        }

        public void Stop()
        {
            if (AudioSource != null && AudioSource.isPlaying)
            {
                TargetVolume = 0.0f;
                currentMultiplier = stopMultiplier;
            }
        }

        public void Update()
        {
            if (AudioSource != null && AudioSource.isPlaying &&
                (AudioSource.volume = Mathf.Lerp(AudioSource.volume, TargetVolume, Time.deltaTime / currentMultiplier)) == 0.0f)
            {
                AudioSource.Stop();
            }
        }
    }

    /// <summary>
    /// Script for objects such as wall of fire that never expire unless manually stopped
    /// </summary>
    public class FireConstantBaseScript : FireBaseScript
    {
        [HideInInspector]
        public LoopingAudioSource m_LoopingAudioSource;
        public float m_TargetVolume = 1f;

        protected override void Awake()
        {
            base.Awake();

            // constant effect, so set the duration really high and add an infinite looping sound
            m_LoopingAudioSource = new LoopingAudioSource(this, AudioSource, StartTime, StopTime);
            m_LoopingAudioSource.TargetVolume = m_TargetVolume;
            Duration = 999999999;
        }

        protected override void Update()
        {
            base.Update();

            m_LoopingAudioSource.Update();
        }

        protected override void Start()
        {
        }

        public void StartFire()
        {
            base.Start();

            m_LoopingAudioSource.Play();
        }

        public override void Stop()
        {
            m_LoopingAudioSource.Stop();

            base.Stop();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarletVOPlayer : MonoBehaviour {

    public enum Version {City, Forest, Cave, Church }

    protected static ScarletVOPlayer _Instance;

    public static ScarletVOPlayer Instance {
        get
        {
            return _Instance;
        }
    }

    protected static string[] s_SoundFiles = {"scarlet_city", "scarlet_forest", "scarlet_cave", "scarlet_church" };

    protected static float[][][] s_HitSounds =
    {
        new float[][] // city
        {

        },
        new float[][] // forest
        {

        },
        new float[][] // cave
        {

        },
        new float[][] // church
        {

        }
    };
    protected FancyAudioRandomClip s_HitSoundsPlayer;

    public Version m_Version;
    

    private void Start()
    {
        if (_Instance == null)
            _Instance = this;

        SetupPlayers();
    }

    protected void SetupPlayers()
    {
        s_HitSoundsPlayer = new FancyAudioRandomClip(s_HitSounds[(int)m_Version], transform, s_SoundFiles[(int)m_Version], 0.7f);
    }

    public void PlayHitSound()
    {
        // s_HitSoundsPlayer.PlayRandomSound();
    }

}

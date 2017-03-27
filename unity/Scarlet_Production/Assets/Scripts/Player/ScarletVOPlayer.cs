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

    protected static float[][][] s_HitSoundsLight =
    {
        new float[][] // city
        {
            new float[] {15.4f, 16f},
            new float[] {16.1f, 16.6f},
            new float[] {16.7f, 17.2f},
            new float[] {17.3f, 17.8f},
        },
        new float[][] // forest
        {
            new float[] {27.1f, 28.2f},
            new float[] {28.7f, 29.7f},
            new float[] {30.7f, 31.6f},
            new float[] {32.9f, 33.8f},
        },
        new float[][] // cave
        {
            new float[] {33.7f, 34.7f},
            new float[] {34.9f, 36.1f},
            new float[] {36.4f, 37.7f},
        },
        new float[][] // church
        {
            new float[] {78.3f, 79.1f},
            new float[] {79.6f, 80.1f},
            new float[] {80.6f, 81.3f},
        }
    };
    protected FancyAudioRandomClip m_HitSoundsLightPlayer;
    protected IEnumerator m_HitSoundsLightPause;

    protected static float[][][] s_HitSoundsHeavy =
    {
        new float[][] // city
        {
            new float[] {18.7f, 19.7f},
            new float[] {19.8f, 20.8f},
            new float[] {23.4f, 24.6f},
        },
        new float[][] // forest
        {
            new float[] {35.8f, 37.1f},
            new float[] {37.2f, 38.6f},
            new float[] {39.0f, 40.1f},
        },
        new float[][] // cave
        {
            new float[] {37.8f, 39.3f},
            new float[] {39.5f, 40.6f},
            new float[] {42.3f, 43.2f},
        },
        new float[][] // church
        {
            new float[] {73.7f, 75.1f},
            new float[] {75.9f, 77.3f},
        }
    };
    protected FancyAudioRandomClip m_HitSoundsHeavyPlayer;
    protected IEnumerator m_HitSoundsHeavyPause;

    protected static float[][][] s_AttackSounds =
    {
        new float[][] // city
        {
            new float[] {11.9f, 12.5f},
            new float[] {12.7f, 13.3f},
            new float[] {14.0f, 14.5f},
            new float[] {14.6f, 15.3f},
        },
        new float[][] // forest
        {
            new float[] {21.9f, 22.6f},
            new float[] {22.7f, 23.3f},
            new float[] {23.6f, 24.1f},
            new float[] {24.2f, 24.8f},
        },
        new float[][] // cave
        {
            new float[] {16.2f, 16.7f},
            new float[] {17.0f, 17.7f},
            new float[] {17.8f, 18.3f},
            new float[] {19.9f, 20.7f},
        },
        new float[][] // church
        {
            new float[] {18.6f, 19.6f},
            new float[] {20.16f, 20.9f},
            new float[] {21.4f, 22.3f},
        }
    };
    protected FancyAudioRandomClip s_AttackSoundsPlayer;
    protected IEnumerator m_AttackSoundsPause;

    protected static float[][][] s_StaggerSounds =
    {
        new float[][] // city
        {
            new float[] {29f, 31.7f}
        },
        new float[][] // forest
        {
            new float[] {47.0f, 49.0f},
            new float[] {50.5f, 52.1f},
        },
        new float[][] // cave
        {
            new float[] {52.6f, 53.5f}
        },
        new float[][] // church
        {
            new float[] {83.4f, 84.1f},
        }
    };
    protected FancyAudioRandomClip s_StaggerSoundsPlayer;

    protected static float[][] s_DeathSounds =
    {
        new float[]{33.3f, 35.4f}, // city
        new float[]{58.4f, 61.1f}, // forest
        new float[]{21.0f, 23.6f}, // cave
        new float[]{23.1f, 25.6f}  // church
    };

    protected static float[][] s_VictorySounds =
    {
        new float[]{35.4f, 42.4f}, // city
        new float[]{72.7f, 79.1f}, // forest
        new float[]{72.8f, 81.3f, 81.6f, 88.5f}, // cave
        new float[]{99.1f, 105.1f}  // church
    };

    protected static float[][] m_BadlyWoundedSounds =
    {
        new float[]{0f, 11.6f}, // city
        new float[]{3f, 13.3f}, // forest
        new float[]{0f, 16.0f}, // cave
        new float[]{0, 16.5f}  // church
    };
    protected FARQ m_BadlyWoundedSoundsPlayer;

    public Version m_Version;
    

    private void Start()
    {
        if (_Instance == null)
            _Instance = this;

        SetupPlayers();
    }

    public void SetupPlayers()
    {
        m_HitSoundsLightPlayer = new FancyAudioRandomClip(s_HitSoundsLight[(int)m_Version], transform, s_SoundFiles[(int)m_Version], 0.7f);
        m_HitSoundsHeavyPlayer = new FancyAudioRandomClip(s_HitSoundsHeavy[(int)m_Version], transform, s_SoundFiles[(int)m_Version], 0.7f);
        s_AttackSoundsPlayer = new FancyAudioRandomClip(s_AttackSounds[(int)m_Version], transform, s_SoundFiles[(int)m_Version], 0.7f);
        s_StaggerSoundsPlayer = new FancyAudioRandomClip(s_StaggerSounds[(int)m_Version], transform, s_SoundFiles[(int)m_Version], 0.7f);
    }

    public void PlayLightHitSound()
    {
        if (m_HitSoundsLightPause != null)
            return;

        m_HitSoundsLightPlayer.PlayRandomSound();
        m_HitSoundsLightPause = LightHitSoundPause();
        StartCoroutine(m_HitSoundsLightPause);
    }

    protected IEnumerator LightHitSoundPause()
    {
        yield return new WaitForSeconds(0.8f);
        m_HitSoundsLightPause = null;
    }

    public void PlayHeavyHitSound()
    {
        if (m_HitSoundsHeavyPause != null)
            return;

        m_HitSoundsHeavyPlayer.PlayRandomSound();
        m_HitSoundsHeavyPause = HeavyHitSoundPause();
        StartCoroutine(m_HitSoundsHeavyPause);
    }

    protected IEnumerator HeavyHitSoundPause()
    {
        yield return new WaitForSeconds(0.8f);
        m_HitSoundsHeavyPause = null;
    }

    public void PlayAttackSound()
    {
        if (m_AttackSoundsPause != null)
            return;

        s_AttackSoundsPlayer.PlayRandomSound();
        m_AttackSoundsPause = AttackSoundPause();
        StartCoroutine(m_AttackSoundsPause);
    }

    protected IEnumerator AttackSoundPause()
    {
        yield return new WaitForSeconds(1.5f);
        m_AttackSoundsPause = null;
    }

    public void PlayStaggerSound()
    {
        s_StaggerSoundsPlayer.PlayRandomSound();
    } 

    public void PlayVictorySound()
    {
        float[] times = s_VictorySounds[(int)m_Version];
        new FARQ().ClipName(s_SoundFiles[(int)m_Version]).StartTime(times[0]).EndTime(times[1]).Location(transform).Volume(0.7f).PlayUnlessPlaying();
    }

    public void PlaySpecialFairyVictorySound()
    {
        float[] times = s_VictorySounds[(int)m_Version];
        new FARQ().ClipName(s_SoundFiles[(int)m_Version]).StartTime(times[2]).EndTime(times[3]).Location(transform).Volume(0.7f).PlayUnlessPlaying();
    }

    public void PlayDeathSound()
    {
        float[] times = s_DeathSounds[(int)m_Version];
        new FARQ().ClipName(s_SoundFiles[(int)m_Version]).StartTime(times[0]).EndTime(times[1]).Location(transform).Volume(0.7f).PlayUnlessPlaying();
    }

    public void PlayBadlyWoundedSound()
    {
        if (m_BadlyWoundedSoundsPlayer != null)
            return;

        ContinuePlayingBadlyWoundedSound();
    }

    public void ContinuePlayingBadlyWoundedSound()
    {
        float[] times = m_BadlyWoundedSounds[(int)m_Version];
        m_BadlyWoundedSoundsPlayer = new FARQ().ClipName(s_SoundFiles[(int)m_Version]).StartTime(times[0]).EndTime(times[1]).Location(transform).Volume(0.350f).OnFinish(ContinuePlayingBadlyWoundedSound);
        m_BadlyWoundedSoundsPlayer.Play();
    }

    public void StopPlayingBadlyWoundedSound()
    {
        if (m_BadlyWoundedSoundsPlayer != null)
        {
            m_BadlyWoundedSoundsPlayer.StopIfPlaying();
            m_BadlyWoundedSoundsPlayer = null;
        }
    }

}

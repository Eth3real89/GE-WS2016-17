using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelSoundPlayer : MonoBehaviour {

    private static AngelSoundPlayer s_Instance;

    protected static float[][] s_LightAttackSounds =
    {
        new float[] {0, 0.9f },
        new float[] {1.5f, 2.7f },
        new float[] {3.2f, 4.1f },
        new float[] {8.3f, 9.6f }
    };
    protected static FancyAudioRandomClip s_LightAttackPlayer;
    protected static IEnumerator s_LightAttackTimer;

    protected static float[][] s_HeavyAttackSounds =
    {
        new float[] {10.7f, 12.4f },
        new float[] {12.6f, 14.5f },
        new float[] {18.5f, 20.4f },
        new float[] {28.9f, 31.1f }
    };
    protected static FancyAudioRandomClip s_HeavyAttackPlayer;

    protected static float[][] s_HitSoundsTypeOne =
    {
        new float[] {46.1f, 46.7f },
        new float[] {46.8f, 47.8f },
        new float[] {47.9f, 48.9f },
    };
    protected static FancyAudioRandomClip s_HitSoundsTypeOnePlayer;
    protected static IEnumerator s_HitSoundsTypeOneTimer;

    protected static float[][] s_HitSoundsTypeTwo =
    {
        new float[] {0.5f, 1.8f },
        new float[] {2.55f, 4.2f },
    };
    protected static FancyAudioRandomClip s_HitSoundsTypeTwoPlayer;
    protected static IEnumerator s_HitSoundsTypeTwoTimer;

    protected static float[][] s_MiscStanceSounds =
    {
        new float[] {0, 1.7f },
        new float[] {4.9f, 5.9f },
    };
    protected static FancyAudioRandomClip s_MiscStancePlayer;

    protected static float[][] s_MiscWindupSounds =
    {
        new float[] {2.1f, 3.6f },
        new float[] {6.2f, 7.7f },
    };
    protected static FancyAudioRandomClip s_MiscWindupPlayer;

    private void Start()
    {
        s_Instance = this;

        s_LightAttackPlayer = new FancyAudioRandomClip(s_LightAttackSounds, transform, "angel_foley");
        s_HeavyAttackPlayer = new FancyAudioRandomClip(s_HeavyAttackSounds, transform, "angel_foley");
        s_HitSoundsTypeOnePlayer = new FancyAudioRandomClip(s_HitSoundsTypeOne, transform, "angel_foley");
        s_HitSoundsTypeTwoPlayer = new FancyAudioRandomClip(s_HitSoundsTypeTwo, transform, "angel_einstecken");
        s_MiscStancePlayer = new FancyAudioRandomClip(s_MiscStanceSounds, transform, "angel_attack_sounds");
        s_MiscWindupPlayer = new FancyAudioRandomClip(s_MiscWindupSounds, transform, "angel_attack_sounds");
    }

    public static void PlayLightAttackSound()
    {
        //if (s_LightAttackTimer != null)
        //    return;

        s_LightAttackPlayer.PlayRandomSound();
        s_LightAttackTimer = WaitSomeTime(1f);
        s_Instance.StartCoroutine(s_LightAttackTimer);
    }

    public static void PlayHeavyAttackSound()
    {
        s_HeavyAttackPlayer.PlayRandomSound();
    }

    public static void PlayHitSoundTypeOne()
    {
        if (s_HitSoundsTypeOneTimer != null)
            return;

        s_HitSoundsTypeOnePlayer.PlayRandomSound();
        s_HitSoundsTypeOneTimer = WaitSomeTime(.4f);
        s_Instance.StartCoroutine(s_HitSoundsTypeOneTimer);
    }

    public static void PlayHitSoundTypeTwo()
    {
        if (s_HitSoundsTypeTwoTimer != null)
            return;

        s_HitSoundsTypeTwoPlayer.PlayRandomSound();
        s_HitSoundsTypeTwoTimer = WaitSomeTime(.4f);
        s_Instance.StartCoroutine(s_HitSoundsTypeTwoTimer);
    }

    public static void PlayMiscStanceSound()
    {
        s_MiscStancePlayer.PlayRandomSound();
    }

    public static void PlayMiscWindupSound()
    {
        s_MiscWindupPlayer.PlayRandomSound();
    }

    protected static IEnumerator WaitSomeTime(float time)
    {
        yield return new WaitForSeconds(time);
    }

}

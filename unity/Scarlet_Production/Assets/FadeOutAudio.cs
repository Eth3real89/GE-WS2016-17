using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutAudio : MonoBehaviour
{
    public void FadeOut()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        AudioSource s = GetComponent<AudioSource>();
        LerpTimer timer = new LerpTimer(10);
        float startVol = s.volume;

        while (timer.GetLerpProgress() < 1)
        {
            s.volume = Mathf.Lerp(startVol, 0, timer.GetLerpProgress());
            yield return null;
        }
    }
}

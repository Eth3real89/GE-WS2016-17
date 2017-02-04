using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFlicker : MonoBehaviour
{

    public void InitFlicker()
    {
        StartCoroutine(FlickerLight());
        StartCoroutine(FlickerSound());
    }

    IEnumerator FlickerLight()
    {
        Animation[] animations = GetComponentsInChildren<Animation>();
        for (int i = 0; i < animations.Length; i = i + 3)
        {
            animations[i].Play();
            animations[i + 1].Play();
            animations[i + 2].Play();
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }

    IEnumerator FlickerSound()
    {
        AudioSource[] audio = GetComponentsInChildren<AudioSource>();
        for (int i = 0; i < audio.Length; i = i + 1)
        {
            audio[i].Play();
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }
}

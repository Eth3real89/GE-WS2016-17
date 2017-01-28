using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFlicker : MonoBehaviour
{

    public void InitFlicker()
    {
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        Animation[] animations = GetComponentsInChildren<Animation>();
        for (int i = 0; i < animations.Length; i = i + 2)
        {
            animations[i].Play();
            animations[i + 1].Play();
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }
}

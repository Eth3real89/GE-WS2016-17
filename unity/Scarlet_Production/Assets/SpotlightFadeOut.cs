using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightFadeOut : MonoBehaviour
{
    private Light light;

    private void Start()
    {
        light = GetComponent<Light>();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        float startIntensity = light.intensity;
        LerpTimer t = new LerpTimer(2f);
        t.Start();
        while (t.GetLerpProgress() < 1)
        {
            light.intensity = Mathf.Lerp(startIntensity, 0, t.GetLerpProgress());
            yield return null;
        }
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnMoonRed : MonoBehaviour
{
    public float changeSpeed;

    public void StartRedMoon()
    {
        Color white = new Color(0.022f, 0.022f, 0.022f);
        Color red = new Color(0.678f, 0, 0);
        Renderer renderer = GetComponent<Renderer>();
        StartCoroutine(TurnRed(renderer, white, red));
    }

    IEnumerator TurnRed(Renderer renderer, Color white, Color red)
    {
        LerpTimer t = new LerpTimer(changeSpeed);
        t.Start();
        while (t.GetLerpProgress() < 1)
        {
            renderer.material.SetColor("_EmissionColor", Color.Lerp(white, red, t.GetLerpProgress()));
            yield return null;
        }
    }
}

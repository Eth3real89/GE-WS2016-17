using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesRed : MonoBehaviour
{
    public void TurnEyesRed()
    {
        StartCoroutine(FadeEyesRed());
    }

    IEnumerator FadeEyesRed()
    {
        Renderer body = GetComponent<Renderer>();
        LerpTimer t = new LerpTimer(0.2f);
        t.Start();
        while (t.GetLerpProgress() < 1)
        {
            Color c = Color.Lerp(Color.black, Color.white, t.GetLerpProgress());
            body.materials[3].SetColor("_Color", c);
            body.materials[3].SetColor("_EmissionColor", c);
            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesWhite : MonoBehaviour
{
    public int matId = 3;

    public void TurnEyesWhite()
    {
        StartCoroutine(FadeEyesWhite());
    }

    IEnumerator FadeEyesWhite()
    {
        Renderer body = GetComponent<Renderer>();
        LerpTimer t = new LerpTimer(0.2f);
        t.Start();
        while (t.GetLerpProgress() < 1)
        {
            Color c = Color.Lerp(Color.black, Color.white, t.GetLerpProgress());
            body.materials[matId].SetColor("_Color", c);
            body.materials[matId].SetColor("_EmissionColor", c);
            yield return null;
        }
    }
}

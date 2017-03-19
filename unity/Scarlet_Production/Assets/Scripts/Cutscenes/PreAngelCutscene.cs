using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CinematicEffects;

public class PreAngelCutscene : MonoBehaviour
{

    public void AngelVoiceline1()
    {
        new FARQ().ClipName("angel").StartTime(0f).EndTime(14.6f).Location(Camera.main.transform).Play();
    }

    public void AngelVoiceline2()
    {
        new FARQ().ClipName("angel").StartTime(15.6f).EndTime(21.5f).Location(Camera.main.transform).Play();
    }

    public void AngelVoiceline3()
    {
        new FARQ().ClipName("angel").StartTime(21.8f).EndTime(25.5f).Location(Camera.main.transform).Play();
    }

    public void MaxOutBloom()
    {
        StartCoroutine(MaxBloom());
    }

    IEnumerator MaxBloom()
    {
        Bloom bloom = Camera.main.GetComponent<Bloom>();
        while (true)
        {
            bloom.settings.intensity += Time.deltaTime;
            yield return null;
        }
    }
}

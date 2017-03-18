using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        new FARQ().ClipName("angel").StartTime(21.8f).EndTime(26.2f).Location(Camera.main.transform).Play();
    }
}

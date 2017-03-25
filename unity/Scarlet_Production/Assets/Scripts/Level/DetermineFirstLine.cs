using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetermineFirstLine : MonoBehaviour
{
    private TalkWithNpc talkScript;

    private void Start()
    {
        talkScript = GetComponent<TalkWithNpc>();

        int fairies = PlayerPrefs.GetInt("FairiesDefeated", 0);
        int encounter = PlayerPrefs.GetInt("SecondEncounter", 0);

        if (fairies == 1)
            return;

        TalkWithNpc.TimeMarker marker = new TalkWithNpc.TimeMarker();

        if (encounter == 1)
        {
            marker.begin = 106.5f;
            marker.end = 111.9f;
        }
        else
        {
            marker.begin = 99.3f;
            marker.end = 105.2f;
        }

        talkScript.marker[0] = marker;
    }
}

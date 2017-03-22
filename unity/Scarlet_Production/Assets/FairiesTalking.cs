using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairiesTalking : MonoBehaviour
{
    [System.Serializable]
    public struct TimeMarker
    {
        public string id;
        public float begin;
        public float end;
    }

    public Transform fairy, target;
    public List<TimeMarker> marker;
    public float startDelay, delay;

    private int currentLine;

    void Start()
    {
        Invoke("Voiceline", startDelay);
    }

    private void SayNextLine()
    {
        currentLine++;
        if (currentLine >= marker.Count)
            return;

        Invoke("Voiceline", delay);
    }

    private void Voiceline()
    {
        TimeMarker currentMarker = marker[currentLine];
        new FARQ().ClipName(currentMarker.id).StartTime(currentMarker.begin).EndTime(currentMarker.end).Location(Camera.main.transform).OnFinish(SayNextLine).Play();
    }

    public void SetFairyTarget()
    {
        fairy.GetComponent<FairyCircling>().target = target;
    }
}

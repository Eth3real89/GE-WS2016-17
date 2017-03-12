using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkWithNpc : Interactor
{
    [System.Serializable]
    public struct TimeMarker
    {
        public float begin;
        public float end;
    }

    public string audioID;
    public List<TimeMarker> marker;

    private int currentDialog;
    private bool isInteracting;

    public override void Interact()
    {
        if (isInteracting)
            return;
        isInteracting = true;
        new FARQ().ClipName(audioID).StartTime(marker[currentDialog].begin).
            EndTime(marker[currentDialog].end).Location(Camera.main.transform).OnFinish(StopInteraction).Play();

        if (currentDialog + 1 < marker.Count)
            currentDialog++;
    }

    private void StopInteraction()
    {
        isInteracting = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TalkWithNpc : Interactor
{
    [System.Serializable]
    public struct TimeMarker
    {
        public float begin;
        public float end;
        public UnityEvent doAfter;
    }

    public string audioID;
    public List<TimeMarker> marker;

    private int currentDialog;
    private bool isInteracting;
    private UnityEvent currentEvent;

    public override void Interact()
    {
        if (!m_IsInteractible)
            return;
        if (isInteracting)
            return;
        isInteracting = true;
        currentEvent = marker[currentDialog].doAfter;
        new FARQ().ClipName(audioID).StartTime(marker[currentDialog].begin).
            EndTime(marker[currentDialog].end).Location(Camera.main.transform).OnFinish(StopInteraction).Play();

        if (currentDialog + 1 < marker.Count)
            currentDialog++;
    }

    private void StopInteraction()
    {
        currentEvent.Invoke();
        isInteracting = false;
    }
}

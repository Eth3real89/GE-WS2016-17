using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkWithWerewolf : Interactor
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
            currentDialog++;
    }

    private void StopInteraction()
    {
        if (currentDialog == marker.Count)
        {
            GetComponent<Animator>().SetTrigger("Die");
            Destroy(this);
        }
        isInteracting = false;
    }
}

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

    public override void Interact()
    {
        GetComponentInChildren<UIItemPickupController>().OnItemPickedUp();
    }
}

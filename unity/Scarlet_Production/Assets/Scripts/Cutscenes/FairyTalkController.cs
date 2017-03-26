using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyTalkController : MonoBehaviour
{
    [System.Serializable]
    public struct TimeMarker
    {
        public float begin;
        public float end;
        public float extraDelay;
        public string audioId;
    }

    public List<TimeMarker> marker;
    public FairyCircling[] fairies;
    public Transform fleePoint;

    private int markerId;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            foreach (FairyCircling fc in fairies)
                fc.target = other.transform;
            TriggerVoiceline();
        }
    }

    private void TriggerVoiceline()
    {
        if (markerId >= marker.Count)
        {
            foreach (FairyCircling fc in fairies)
                fc.target = fleePoint;
            return;
        }
        StartCoroutine(FairySpeech());
    }

    IEnumerator FairySpeech()
    {
        TimeMarker currentMarker = marker[markerId];
        yield return new WaitForSeconds(currentMarker.extraDelay);
        new FARQ().ClipName(currentMarker.audioId).OnFinish(TriggerVoiceline).StartTime(currentMarker.begin).EndTime(currentMarker.end).Location(Camera.main.transform).Play();
        markerId++;
    }
}

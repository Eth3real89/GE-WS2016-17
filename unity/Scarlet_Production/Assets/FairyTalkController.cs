﻿using System.Collections;
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
    private int markerId;

    private void Start()
    {
        if (markerId >= marker.Count)
            return;
        StartCoroutine(FairySpeech());
    }

    IEnumerator FairySpeech()
    {
        TimeMarker currentMarker = marker[markerId];
        yield return new WaitForSeconds(currentMarker.extraDelay);
        new FARQ().ClipName(currentMarker.audioId).OnFinish(Start).StartTime(currentMarker.begin).EndTime(currentMarker.end).Location(Camera.main.transform).Play();
        markerId++;
    }
}

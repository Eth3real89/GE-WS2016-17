using SequencedActionCreator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroducePortal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SequencedActionController.FinishedCutsceneCallback callback = DestroyField;

        if (other.CompareTag("Player"))
        {
            SequencedActionController.Instance.PlayCutscene("IntroduceGuide", callback);
        }
    }

    public void PlayGuideVO()
    {
        new FARQ().ClipName("theguide").StartTime(6.8f).EndTime(10.2f).Location(Camera.main.transform).Play();
    }

    public void DestroyField()
    {
        Destroy(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideCalling : MonoBehaviour
{
    private bool m_IsTalking;

    private void OnTriggerExit(Collider other)
    {
        if (m_IsTalking)
            return;

        m_IsTalking = true;
        new FARQ().ClipName("theguide").StartTime(10).EndTime(14).Location(Camera.main.transform).OnFinish(TalkAgain).Play();
    }

    private void TalkAgain()
    {
        m_IsTalking = false;
    }
}

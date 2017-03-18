using SequencedActionCreator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantlyPlayCutscene : MonoBehaviour
{
    public string m_CutsceneName;

    private void Start()
    {
        SequencedActionController.Instance.PlayCutscene(m_CutsceneName);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyAudioRegisterClips : MonoBehaviour {

    public AudioClip m_Clip;
    public string m_ClipName;

    private void Start()
    {
        StartCoroutine(RegisterOnLoad());
    }

    protected IEnumerator RegisterOnLoad()
    {
        while(m_Clip.loadState == AudioDataLoadState.Loading)
        {
            yield return null;
        }

        while (FancyAudio.Instance == null)
        {
            yield return null;
        }

        FancyAudio.Instance.RegisterClip(m_ClipName, m_Clip);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEFairyBlastWaveHintEffect : PlayableEffect
{

    public GameObject m_BlastWaveTelegraphingEffect;

    public override void Play(Vector3 position = default(Vector3))
    {
        Component[] comps = m_BlastWaveTelegraphingEffect.GetComponentsInChildren<ParticleSystem>();

        foreach(ParticleSystem ps in comps)
        {
            ps.Play();
        }

        StartCoroutine(HideEffect());
    }

    public override void Hide()
    {
        Component[] comps = m_BlastWaveTelegraphingEffect.GetComponentsInChildren<ParticleSystem>();

        foreach(ParticleSystem ps in comps)
        {
            ps.Stop();
        }
    }

    private IEnumerator HideEffect()
    {
        yield return new WaitForSeconds(0.3f);

        Hide();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEFairyBulletHintEffect : PlayableEffect
{
    public GameObject m_BulletTelegraphingEffect;

    public override void Play(Vector3 position = default(Vector3))
    {
        Component[] comps = m_BulletTelegraphingEffect.GetComponentsInChildren<ParticleSystem>();

        foreach(ParticleSystem ps in comps)
        {
            ps.Play();
        }

        StartCoroutine(HideEffect());
    }

    public override void Hide()
    {
        Component[] comps = m_BulletTelegraphingEffect.GetComponentsInChildren<ParticleSystem>();

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

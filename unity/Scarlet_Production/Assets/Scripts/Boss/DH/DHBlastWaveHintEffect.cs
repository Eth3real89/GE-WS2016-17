using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DHBlastWaveHintEffect : PlayableEffect
{
    public GameObject m_BlastWaveTelegraphingEffect;

    public override void Play(Vector3 position = default(Vector3))
    {
        GameObject copy = Instantiate(m_BlastWaveTelegraphingEffect);
        copy.transform.position = position * 1f;
        copy.SetActive(true);
        Component[] comps = copy.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem ps in comps)
        {
            ps.Play();
        }

        StartCoroutine(HideEffect(copy));
    }

    public override void Hide()
    {
        // Either: Keep Track of Instances & Hide them all
        // Or: Ignore, who cares
        /*Component[] comps = m_BlastWaveTelegraphingEffect.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in comps)
        {
            ps.Stop();
        }*/
    }

    private IEnumerator HideEffect(GameObject copy)
    {
        yield return new WaitForSeconds(0.3f);
        try
        {
            if (copy == null)
                yield break;

            Component[] comps = copy.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in comps)
            {
                ps.Stop();
            }
        }
        catch { }
    }
}

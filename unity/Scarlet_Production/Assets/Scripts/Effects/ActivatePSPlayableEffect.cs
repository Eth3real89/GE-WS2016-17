using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***
 * Class that activates all particle system that are its children.
 */
public class ActivatePSPlayableEffect : PlayableEffect
{
    protected ParticleSystem[] m_Systems;
    public float m_DisableAfter = -1;

    protected IEnumerator m_Timer;

    private void Start()
    {
        m_Systems = GetComponentsInChildren<ParticleSystem>();
        SetStatus(false);
    }

    public override void Play(Vector3 position = default(Vector3))
    {
        SetStatus(true);

        if (m_DisableAfter >= 0)
        {
            if (m_Timer != null)
                StopCoroutine(m_Timer);

            m_Timer = DisableAfterWaiting();
            StartCoroutine(m_Timer);
        }
    }

    public override void Hide()
    {
        SetStatus(false);
    }

    protected IEnumerator DisableAfterWaiting()
    {
        yield return new WaitForSeconds(m_DisableAfter);
        SetStatus(false);

        m_Timer = null;
    }

    protected virtual void SetStatus(bool on)
    {
        foreach(ParticleSystem ps in m_Systems)
        {
            if (on)
            {
                ps.Play();
            }
            else
            {
                ps.Stop();
            }
        }
    }

}

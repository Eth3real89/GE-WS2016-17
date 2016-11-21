using UnityEngine;
using System.Collections;
using System;

public abstract class AEAttackSeries : Attack {

    public AEAttackPart[] m_Parts;
    public bool m_Cancelled = false;

    public MonoBehaviour m_Behaviour;

    public AEAttackSeries(MonoBehaviour behaviour)
    {
        this.m_Behaviour = behaviour;
    }

    public abstract void BeforeSeries(Transform bossTransform);

    public void RunSeries (Transform bossTransform)
    {
        BeforeSeries(bossTransform);

        for (int i = 0; i < m_Parts.Length; i++)
        {
            m_Behaviour.StartCoroutine(StartPartAfter(m_Parts[i], bossTransform));
        }
    }

    private IEnumerator StartPartAfter(AEAttackPart part, Transform bossTransform)
    {
        yield return new WaitForSeconds(part.delay);

        if (!m_Cancelled)
            part.LaunchPart();
    }
}

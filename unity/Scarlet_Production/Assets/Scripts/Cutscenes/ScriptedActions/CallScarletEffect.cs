using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallScarletEffect : MonoBehaviour
{
    private Light m_Light;
    private float m_StartingRange;

    private void Start()
    {
        m_Light = GetComponent<Light>();
        m_StartingRange = m_Light.range;
    }

    private void OnEnable()
    {
        EffectController.EnteredOtherWorld += CallScarlet;
    }

    private void OnDisable()
    {
        EffectController.EnteredOtherWorld -= CallScarlet;
    }

    private void CallScarlet()
    {
        StartCoroutine(LightBurst());
    }

    IEnumerator LightBurst()
    {
        LerpTimer timer = new LerpTimer();
        timer.Start(1f);
        while (m_Light.range != 1)
        {
            m_Light.range = Mathf.Lerp(m_StartingRange, 1, timer.GetLerpProgress());
            yield return null;
        }
        timer.Start(1f);
        while (m_Light.range != 3.5f)
        {
            m_Light.range = Mathf.Lerp(1, 3.5f, timer.GetLerpProgress());
            yield return null;
        }
    }
}

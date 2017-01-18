using System.Collections;
using UnityStandardAssets.CinematicEffects;
using UnityEngine;

public class EffectController : GenericSingletonClass<EffectController>
{
    public LensFlare m_MoonFlare;
    public Bloom m_Bloom;

    private Bloom.Settings m_DefaultBloomSettings;
    private Coroutine m_CurrentCoroutine;
    private LerpTimer m_LerpTimer;

    private void Start()
    {
        m_LerpTimer = new LerpTimer();
        m_DefaultBloomSettings = m_Bloom.settings;
    }

    private void StopCurrentCoroutine()
    {
        if (m_CurrentCoroutine != null)
            StopCoroutine(m_CurrentCoroutine);
    }

    public void EnterStrongLight()
    {
        StopCurrentCoroutine();
        m_CurrentCoroutine = StartCoroutine(FadeBloom(5, 5));
    }

    public void ExitStrongLight()
    {
        StopCurrentCoroutine();
        m_CurrentCoroutine = StartCoroutine(FadeBloom(m_DefaultBloomSettings.intensity, m_DefaultBloomSettings.radius));
    }

    IEnumerator FadeBloom(float intensity, float radius)
    {
        m_LerpTimer.Start(0.25f);
        float startIntensity = m_Bloom.settings.intensity;
        float startRadius = m_Bloom.settings.radius;

        while (m_Bloom.settings.intensity != intensity)
        {
            m_Bloom.settings.intensity = Mathf.Lerp(startIntensity, intensity, m_LerpTimer.GetLerpProgress());
            m_Bloom.settings.radius = Mathf.Lerp(startRadius, radius, m_LerpTimer.GetLerpProgress());
            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSaveSignController : MonoBehaviour {

    public delegate void SaveSignCallback();

    private Image m_SaveSign;

    private bool m_WaitTillFadeOut = false;

    private float m_TimeToFadeOriginal = 1f;
    private float m_TimeToShowOriginal = 4f;

    private float m_TimeToFade;
    private float m_TimeToShow;

    private IEnumerator m_TutorialEnumerator;
    private SaveSignCallback m_SaveSignCallback;

    // Use this for initialization
    void Start () {
        m_SaveSign = GetComponentInChildren<Image>();

    }
	
	// Update is called once per frame
	void Update () {
        if (m_WaitTillFadeOut)
        {
            m_TimeToShow -= Time.deltaTime;
            if (m_TimeToShow <= 0)
            {
                m_WaitTillFadeOut = false;
                if (m_TutorialEnumerator != null)
                    StopCoroutine(m_TutorialEnumerator);

                m_TutorialEnumerator = FadeTo(0.0f, m_TimeToFade, true);
                StartCoroutine(m_TutorialEnumerator);
            }
        }
    }

    private void ResetValues()
    {
        m_WaitTillFadeOut = false;

        m_TimeToFade = m_TimeToFadeOriginal;
        m_TimeToShow = m_TimeToShowOriginal;
    }

    public void FadeInSaveSign(SaveSignCallback callback = null)
    {
        m_SaveSignCallback = callback;
        ResetValues();
        if (m_TutorialEnumerator != null)
            StopCoroutine(m_TutorialEnumerator);
        m_TutorialEnumerator = FadeTo(1.0f, m_TimeToFade, false);
        StartCoroutine(m_TutorialEnumerator);
    }

    IEnumerator FadeTo(float aValue, float aTime, bool callback)
    {
        float alpha = m_SaveSign.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newBlack = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            m_SaveSign.color = newBlack;
            yield return null;
        }

        m_SaveSign.color = new Color(1, 1, 1, aValue);
        if (aValue != 0)
        {
            m_WaitTillFadeOut = true;
        }
        if(callback && m_SaveSignCallback != null) 
        {
            m_SaveSignCallback();
        }
    }
}

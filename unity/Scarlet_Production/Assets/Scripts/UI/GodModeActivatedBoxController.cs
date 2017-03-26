using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodModeActivatedBoxController : MonoBehaviour
{
    public GameObject m_ScreenContainer;

    private Image m_Background;
    private Text m_Message;
    private bool m_ShowScreen = false;
    private bool m_WaitTillFadeOut = false;

    private float m_TimeToFadeOriginal = 1f;
    private float m_TimeToShowOriginal = 3f;

    private float m_TimeToFade;
    private float m_TimeToShow;

    private int m_CountToShowMax = 2;
    private int m_CountToShow = 1;
    private string m_Message1;
    private string m_Message2;


    private IEnumerator m_TutorialEnumerator;

    // Use this for initialization
    void Start()
    {
        m_Background = m_ScreenContainer.GetComponentInChildren<Image>();
        m_Message = m_ScreenContainer.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_WaitTillFadeOut && m_CountToShow <= m_CountToShowMax)
        {
            m_TimeToShow -= Time.deltaTime;
            if (m_TimeToShow <= 0)
            {
                m_WaitTillFadeOut = false;
                m_CountToShow += 1;
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
        m_CountToShow = 1;
        if(m_Message2 == "")
        {
            m_CountToShowMax = 1;
        } else
        {
            m_CountToShowMax = 2;
        }

        m_TimeToFade = m_TimeToFadeOriginal;
        m_TimeToShow = m_TimeToShowOriginal;
    }

    public void ShowGodModeAcitvated(string message1, string message2)
    {
        m_Message1 = message1;
        m_Message2 = message2;

        ResetValues();
        FadeIn();
    }

    private void FadeIn()
    {
        if(m_CountToShow == 2)
        {
            m_Message.text = m_Message2;
        } else if(m_CountToShow == 1)
        {
            m_Message.text = m_Message1;
        }

        if (m_TutorialEnumerator != null)
            StopCoroutine(m_TutorialEnumerator);
        m_TutorialEnumerator = FadeTo(1.0f, m_TimeToFade, false);
        StartCoroutine(m_TutorialEnumerator);
    }

    IEnumerator FadeTo(float aValue, float aTime, bool fadeOut)
    {
        float alpha = m_Background.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newBlack = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            m_Background.color = newBlack;
            m_Message.color = newBlack;
            yield return null;
        }

        m_Background.color = new Color(1, 1, 1, aValue);
        m_Message.color = new Color(1, 1, 1, aValue);
        if (aValue != 0)
        {
            m_WaitTillFadeOut = true;
        }
        if(fadeOut && m_CountToShow <= m_CountToShowMax)
        {
            FadeIn();
        }
    }

}

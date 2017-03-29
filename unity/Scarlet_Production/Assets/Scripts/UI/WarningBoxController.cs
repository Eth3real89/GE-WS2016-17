using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WarningBoxController : MonoBehaviour {


    public delegate void WarningBoxConfirmedCallback();
    public delegate void WarningBoxDeclinedCallback();


    public Image m_Background;
    public Text m_Message;
    public Image m_CloseHintConfirmAura;
    public Image m_CloseHintConfirmButton;
    public Text m_CloseHintConfirmText;
    public Image m_CloseHintDeclineAura;
    public Image m_CloseHintDeclineButton;
    public Text m_CloseHintDeclineText;

    public float m_InteractTime = 0.75f;

    private IEnumerator m_TutorialEnumerator;

    private bool m_FadeIn = false;
    private bool m_FadeOut = false;
    private float m_InteractTimeDeclineCurrent = 0f;
    private float m_InteractTimeConfirmCurrent = 0f;

    private WarningBoxConfirmedCallback m_ConfirmedCallback;
    private WarningBoxDeclinedCallback m_DeclinedCallback;
   

	// Update is called once per frame
	void Update () {

        if (m_FadeIn)
        {
            //stop movements at fade start
            SetScarletControlsEnabled(false);
            m_FadeIn = false;
            if (m_TutorialEnumerator != null)
                StopCoroutine(m_TutorialEnumerator);

            m_TutorialEnumerator = FadeTo(1.0f, 0.3f, false, false);
            StartCoroutine(m_TutorialEnumerator);
        } else if (Input.GetButton("Attack") && m_FadeOut)
        {
            m_InteractTimeConfirmCurrent += Time.deltaTime;
            UpdateConfirmClose(m_InteractTimeConfirmCurrent / m_InteractTime);
        }
        else if (Input.GetButton("Parry") && m_FadeOut)
        {
            m_InteractTimeDeclineCurrent += Time.deltaTime;
            UpdateDeclineClose(m_InteractTimeDeclineCurrent / m_InteractTime);

        }
        else if (m_FadeOut)
        {
            if (m_TutorialEnumerator != null)
                StopCoroutine(m_TutorialEnumerator);

            m_InteractTimeConfirmCurrent = 0;
            m_InteractTimeDeclineCurrent = 0;
            UpdateConfirmClose(0);
            UpdateDeclineClose(0);
        }
    }


    private void UpdateConfirmClose(float floatcurrentTime)
    {
        if (floatcurrentTime >= 1)
        {
            floatcurrentTime = 0;
            m_FadeOut = false;
            if (m_TutorialEnumerator != null)
                StopCoroutine(m_TutorialEnumerator);

            //start movements after fade end (in FadeTo)
            m_TutorialEnumerator = FadeTo(0f, 0.3f, true, false);
            StartCoroutine(m_TutorialEnumerator);
            m_CloseHintDeclineAura.fillAmount = 0;
        }
        m_CloseHintConfirmAura.fillAmount = Mathf.Lerp(0.0f, 1.0f, floatcurrentTime);
    }

    private void UpdateDeclineClose(float floatcurrentTime)
    {
        if (floatcurrentTime >= 1)
        {
            floatcurrentTime = 0;
            m_FadeOut = false;
            if (m_TutorialEnumerator != null)
                StopCoroutine(m_TutorialEnumerator);

            //start movements after fade end (in FadeTo)
            m_TutorialEnumerator = FadeTo(0f, 0.6f, false, true);
            StartCoroutine(m_TutorialEnumerator);
            m_CloseHintConfirmAura.fillAmount = 0;
        }
        m_CloseHintDeclineAura.fillAmount = Mathf.Lerp(0.0f, 1.0f, floatcurrentTime);
    }


    private void SetScarletControlsEnabled(bool enabled)
    {
        PlayerControls controls = FindObjectOfType<PlayerControls>();
        if (controls != null)
        {
            if (enabled)
                controls.EnableAllCommands();
            else
            {
                controls.StopMoving();
                controls.DisableAllCommands();
            }
        }
    }


    IEnumerator FadeTo(float aValue, float aTime, bool confirmedAfter, bool declinedAfter)
    {
        float alpha = m_Background.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColorRed = new Color(0.65f, 0, 0, Mathf.Lerp(alpha, aValue, t));
            Color newColorBlack = new Color(0, 0, 0, Mathf.Lerp(alpha, aValue, t));
            m_CloseHintConfirmText.color = newColorRed;
            m_CloseHintConfirmButton.color = newColorRed;
            m_CloseHintDeclineText.color = newColorRed;
            m_CloseHintDeclineButton.color = newColorRed;
            m_Message.color = newColorRed;
            m_Background.color = newColorBlack;

            yield return null;
        }
        m_CloseHintConfirmButton.color = new Color(0.65f, 0, 0, aValue);
        m_CloseHintDeclineButton.color = new Color(0.65f, 0, 0, aValue);
        m_Background.color = new Color(0, 0, 0, aValue);
        m_CloseHintConfirmText.color = new Color(0.65f, 0, 0, aValue);
        m_CloseHintDeclineText.color = new Color(0.65f, 0, 0, aValue);
        m_Message.color = new Color(0.65f, 0, 0, aValue);

        if (confirmedAfter)
        {
            SetScarletControlsEnabled(true);
            if(m_ConfirmedCallback != null) m_ConfirmedCallback();
        }
        if (declinedAfter)
        {
            SetScarletControlsEnabled(true);
            if (m_DeclinedCallback != null) m_DeclinedCallback();
        }
        if (aValue != 0)
        {
            m_FadeOut = true;
        }
    }

    public void StartFadeIn(string message, string confirmText, string declineText, WarningBoxConfirmedCallback confirmedCallback = null, WarningBoxDeclinedCallback declinedCallback = null)
    {
        m_ConfirmedCallback = confirmedCallback;
        m_DeclinedCallback= declinedCallback;
        m_Message.text = message;
        m_CloseHintConfirmText.text = confirmText;
        m_CloseHintDeclineText.text = declineText;
        m_FadeIn = true;
    }
}

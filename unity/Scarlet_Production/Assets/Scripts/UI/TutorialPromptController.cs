using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPromptController : MonoBehaviour {

    public GameObject TutorialPrompt;
    private Image[] tutorialImages;
    private Text tutorialText;

    private IEnumerator m_TutorialEnumerator;
    protected float m_MaxBackgroundOpacity = 0.5f;

    // Use this for initialization
    void Start () {
        tutorialImages = TutorialPrompt.GetComponentsInChildren<Image>();
        tutorialText = TutorialPrompt.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //ShowTutorial("B", "Parry");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //HideTutorial();
        }
    }

    /// <summary>
    /// Show tutorial instructions with text and button prompt. ButtonShort is A, B, X or Y. Text is the text to show.
    /// </summary>
    /// <param name="buttonShort"></param>
    /// <param name="newText"></param>
    public void ShowTutorial(string buttonShort, string newText, float timeMultplier = 1f)
    {
        tutorialText.text = newText;
        tutorialImages[1].sprite = Resources.Load<Sprite>("controls-" + buttonShort + "_rot");

        if (m_TutorialEnumerator != null)
            StopCoroutine(m_TutorialEnumerator);

        m_TutorialEnumerator = FadeTo(1.0f, 0.2f * timeMultplier);
        StartCoroutine(m_TutorialEnumerator);
    }

    /// <summary>
    /// Hide tutorial instructions
    /// </summary>
    public void HideTutorial(float timeMultiplier = 1f)
    {
        if (m_TutorialEnumerator != null)
            StopCoroutine(m_TutorialEnumerator);

        m_TutorialEnumerator = FadeTo(0.0f, 0.2f * timeMultiplier);
        StartCoroutine(m_TutorialEnumerator);
    }


    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = tutorialText.color.a;
        float alphaBlack = tutorialImages[0].color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newBlack = new Color(0, 0, 0, Mathf.Lerp(alphaBlack, aValue * m_MaxBackgroundOpacity, t));
            Color newColorRed = new Color(0.65f, 0, 0, Mathf.Lerp(alpha, aValue, t));
            tutorialImages[0].color = newBlack;
            tutorialImages[1].color = newColorRed;
            tutorialText.color = newColorRed;

            yield return null;
        }
        tutorialImages[0].color = new Color(0, 0, 0, aValue * m_MaxBackgroundOpacity);
        tutorialImages[1].color = new Color(0.65f, 0, 0, aValue);
        tutorialText.color = new Color(0.65f, 0, 0, aValue);
    }
}

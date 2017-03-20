using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreenController : MonoBehaviour {
	public GameObject m_ScreenContainer;

	private Image m_Background;
	private Text m_Message;
	private bool m_ShowScreen = false;
	private bool m_WaitTillFadeOut = false;

	private float m_TimeToFadeOriginal = 1f;
	private float m_TimeToShowOriginal = 4f;

	private float m_TimeToFade;
	private float m_TimeToShow;

	private GameObject m_CallbackObject;

	private IEnumerator m_TutorialEnumerator;

	// Use this for initialization
	void Start () {
		m_Background = m_ScreenContainer.GetComponentInChildren<Image>();
		m_Message= m_ScreenContainer.GetComponentInChildren<Text>();
	}

	// Update is called once per frame
	void Update () {
		if(m_WaitTillFadeOut)
		{
			m_TimeToShow -= Time.deltaTime;
			if (m_TimeToShow <= 0)
			{
				m_WaitTillFadeOut = false;
				if (m_TutorialEnumerator != null)
					StopCoroutine(m_TutorialEnumerator);

				m_TutorialEnumerator = FadeTo(0.0f, m_TimeToFade);
				StartCoroutine(m_TutorialEnumerator);
			}
		}
	}

	private void ResetValues()
	{
		m_ShowScreen = false;
		m_WaitTillFadeOut = false;

		m_TimeToFade = m_TimeToFadeOriginal;
		m_TimeToShow = m_TimeToShowOriginal;
	}

	public void ShowVictoryScreen(GameObject CallbackObject)
	{
		if(m_CallbackObject == null)
		{
			m_CallbackObject = CallbackObject;
		}
		ResetValues();
		if (m_TutorialEnumerator != null)
			StopCoroutine(m_TutorialEnumerator);
		m_TutorialEnumerator = FadeTo(1.0f, m_TimeToFade);
		StartCoroutine(m_TutorialEnumerator);
	}

	IEnumerator FadeTo(float aValue, float aTime)
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
		} else
		{
			BossFight[] bossfights = GetComponents<BossFight>();
			foreach (BossFight fight in bossfights)
			{
				if (fight.enabled)
				{
                    fight.LoadSceneAfterBossfight();
				}
			}
		}
	}

}

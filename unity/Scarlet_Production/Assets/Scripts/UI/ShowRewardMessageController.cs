using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowRewardMessageController : MonoBehaviour {

	public Image m_Background;
	public Text m_Message;
	public Image m_CloseHintAura;
	public Image m_CloseHintButton;
	public Text m_CloseHintText;
	public bool m_BossfightAfter = false;

	private IEnumerator m_TutorialEnumerator;

	private bool m_FadeIn = false;
	private bool m_FadeOut = false;
	private float m_Time = 0f;
	private float m_TimeMax = 1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(m_FadeIn)
		{
			//stop movements at fade start
			SetScarletControlsEnabled(false);
			m_FadeIn = false;
			if (m_TutorialEnumerator != null)
				StopCoroutine(m_TutorialEnumerator);

			m_TutorialEnumerator = FadeTo(1.0f, 0.6f, false);
			StartCoroutine(m_TutorialEnumerator);
		}
		if (Input.GetButton("Attack") && m_FadeOut)
		{
			m_Time += Time.deltaTime;
			UpdateClose(m_Time / m_TimeMax);
			
		} else if(m_FadeOut)
		{
			if (m_TutorialEnumerator != null)
				StopCoroutine(m_TutorialEnumerator);
			m_Time = 0;
			UpdateClose(0);

		}
	}

	private void UpdateClose(float floatcurrentTime)
	{
		if(floatcurrentTime >= 1)
		{
			floatcurrentTime = 0;
			m_FadeOut = false;
			if (m_TutorialEnumerator != null)
				StopCoroutine(m_TutorialEnumerator);

			//start movements after fade end (in FadeTo)
			m_TutorialEnumerator = FadeTo(0f, 0.6f, true);
			StartCoroutine(m_TutorialEnumerator);
		}
		m_CloseHintAura.fillAmount = Mathf.Lerp(0.0f, 1.0f, floatcurrentTime);
	}

	private void SetScarletControlsEnabled(bool enabled)
	{
		PlayerControls controls = FindObjectOfType<PlayerControls>();
		if (controls != null)
		{
			if (enabled)
				controls.EnableAllCommands();
			else
				controls.DisableAllCommands();
		}
	}

	IEnumerator FadeTo(float aValue, float aTime, bool allowMoveAfter)
	{
		float alpha = m_Background.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColorRed = new Color(0.65f, 0, 0, Mathf.Lerp(alpha, aValue, t));
			Color newColorBlack = new Color(0, 0, 0, Mathf.Lerp(alpha, aValue, t));
			m_CloseHintText.color = newColorRed;
			m_CloseHintButton.color = newColorRed;
			m_Message.color = newColorRed;
			m_Background.color = newColorBlack;

			yield return null;
		}
		m_CloseHintButton.color = new Color(0.65f, 0, 0, aValue);
		m_Background.color = new Color(0, 0, 0, aValue);
		m_CloseHintText.color = new Color(0.65f, 0, 0, aValue);
		m_Message.color = new Color(0.65f, 0, 0, aValue);
		if(allowMoveAfter)
		{
			SetScarletControlsEnabled(allowMoveAfter);
			if(m_BossfightAfter)
			{
				BossFight[] bossfights = FindObjectsOfType<BossFight>();
				foreach (BossFight fight in bossfights)
				{
					if (fight.enabled)
					{
						fight.LoadSceneAfterBossfight();
					}
				}
			}
		} else
		{
			m_FadeOut = true;
		}
	}


	public void StartFadeIn(string message)
	{
		m_Message.text = message;
		m_FadeIn = true;
	}
}

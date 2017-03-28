using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AreaEnterTextController : MonoBehaviour {

	public GameObject textHint;
	public int timeShowHint;
	public int delay;
	public AudioClip m_AreaEnterNotification;

	private bool fadeIn = false;
	private bool fadeOut = false;
	private float time = 0;

	private Text m_Name;
	private Image m_Background;
	private bool m_SoundPlayed = false;

	// Use this for initialization
	void Start ()
	{
		//fadeIn = false;

		m_Name = textHint.GetComponentInChildren<Text>();
		m_Background = textHint.GetComponentInChildren<Image>();
	}

	// Update is called once per frame
	void Update () {
		if(fadeIn)
		{
			time += Time.deltaTime;
			if (time > delay - 1f && !m_SoundPlayed)
			{
				if (m_AreaEnterNotification != null)
					Camera.main.gameObject.AddComponent<AudioSource>().PlayOneShot(m_AreaEnterNotification);
				m_SoundPlayed = true;
			}

			if (time > delay)
			{
				StartCoroutine(FadeTo(1.0f, 0.6f));
				fadeIn = false;
				fadeOut = true;
				time = 0;
				m_SoundPlayed = false;
			}
		}
		if(fadeOut)
		{
			time += Time.deltaTime;
			if (time > timeShowHint)
			{
				fadeOut = false;
				StartCoroutine(FadeTo(0.0f, 0.6f));
			}
		}
	}

	public void StartFadeIn()
	{
		fadeIn = true;
	}

	public void StartFadeInWithText(string areaName, int showDealyTime)
	{
		delay = showDealyTime;
		m_Name.text = areaName;
		fadeIn = true;
	}


	IEnumerator FadeTo(float aValue, float aTime)
	{
		float alpha = 1-aValue;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColorRed = new Color(0.65f, 0, 0, Mathf.Lerp(alpha, aValue, t));
			m_Name.color = newColorRed;
			m_Background.color = newColorRed;

			yield return null;
		}
		m_Background.color = new Color(0.65f, 0, 0, aValue);
		m_Name.color = new Color(0.65f, 0, 0, aValue);

	}

}

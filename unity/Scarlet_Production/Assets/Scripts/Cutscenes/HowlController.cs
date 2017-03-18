using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowlController : MonoBehaviour
{
	public void Talk()
	{
		new FARQ().ClipName("werewolf").StartTime(12.8f).EndTime(28.2f).Location(Camera.main.transform).Play();
	}

	public void Howl()
	{
		GetComponent<Animator>().SetTrigger("HowlTrigger");
		new FARQ().ClipName("werewolf").StartTime(48.2f).EndTime(55.4f).Location(Camera.main.transform).Play();
	}

	public void Enable()
	{
		gameObject.SetActive(true);
	}
}

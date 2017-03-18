using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowlController : MonoBehaviour
{
	public void Howl()
	{
		GetComponent<Animator>().SetTrigger("HowlTrigger");
	}

	public void Enable()
	{
		gameObject.SetActive(true);
	}
}

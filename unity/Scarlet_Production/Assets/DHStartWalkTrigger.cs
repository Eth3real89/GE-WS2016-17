using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DHStartWalkTrigger : MonoBehaviour {

	public void StartWalking()
	{
		GetComponent<Animator>().SetTrigger("WalkTrigger");
	}
}

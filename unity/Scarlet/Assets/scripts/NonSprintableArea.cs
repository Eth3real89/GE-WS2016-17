using UnityEngine;
using System.Collections;

public class NonSprintableArea : MonoBehaviour {
	// for more epicness Scarlet has to walk to the boss
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			other.GetComponent<PlayerControlsCharController>().canSprint = false;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			other.GetComponent<PlayerControlsCharController>().canSprint = true;
		}
	}
}

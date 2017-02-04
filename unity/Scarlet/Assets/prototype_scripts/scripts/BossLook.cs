using UnityEngine;
using System.Collections;

public class BossLook : MonoBehaviour {

	public GameObject playerDummy;
	
	void Update ()
	{
		Vector3 lookPos = playerDummy.transform.position - transform.position;
		lookPos.y = 0;
		transform.rotation = Quaternion.LookRotation(lookPos);
	}
}

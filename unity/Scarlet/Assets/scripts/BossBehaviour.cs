using UnityEngine;
using System.Collections;

public class BossBehaviour : MonoBehaviour {

    private AttackPattern m_AttackPattern;

	// Use this for initialization
	void Start () {
        m_AttackPattern = GetComponent<AttackPattern>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

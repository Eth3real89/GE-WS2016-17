using UnityEngine;
using System.Collections;

public abstract class Attack {

    public float m_Probability;

    public AttackCallbacks m_Callbacks;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public abstract void WhileActive();
}

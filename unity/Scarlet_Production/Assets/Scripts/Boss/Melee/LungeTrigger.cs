using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungeTrigger : Damage {

    public override bool Blockable()
    {
        return false;
    }

    public override float DamageAmount()
    {
        return 45f;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

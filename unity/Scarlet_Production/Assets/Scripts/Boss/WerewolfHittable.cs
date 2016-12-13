using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfHittable : MonoBehaviour, Hittable {

    public CharacterHealth m_Health;

    public void hit(Damage damage)
    {
        m_Health.m_CurrentHealth -= damage.DamageAmount();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

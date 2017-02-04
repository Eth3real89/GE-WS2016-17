using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossCommand : MonoBehaviour {

    public Animator m_Animator;
    public GameObject m_Boss;

    public virtual void InitCommand(GameObject m_Boss, Animator m_Animator)
    {
        this.m_Boss = m_Boss;
        this.m_Animator = m_Animator;
    }

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}
}

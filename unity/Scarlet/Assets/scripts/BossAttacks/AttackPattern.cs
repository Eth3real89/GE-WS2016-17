using UnityEngine;
using System.Collections;

public class AttackPattern : MonoBehaviour {

    public Attack[] m_Attacks;

    public float m_MinTimeBetweenAttacks;
    public float m_MaxTimeBetweenAttacks;

    private float m_AttackActive;
    public Attack m_CurrentAttack;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

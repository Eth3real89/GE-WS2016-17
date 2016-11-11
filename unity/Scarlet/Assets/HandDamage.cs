using UnityEngine;
using System.Collections;

public class HandDamage : MonoBehaviour {

    public bool m_CauseDamage = false;

    public float m_Damage = 20f;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnTriggerEnter(Collider other)
    {
        CheckHit(other);
    }

    void OnTriggerStay(Collider other)
    {
        CheckHit(other);
    }

    private void CheckHit(Collider other)
    {
        if (m_CauseDamage)
        {
            if (GameController.Instance.IsBoss(other.GetComponent<Rigidbody>()))
            {
                GameController.Instance.HitBoss(m_Damage);
                m_CauseDamage = false;
            }
        }
    }



}

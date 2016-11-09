using UnityEngine;
using System.Collections;

public class HandDamage : MonoBehaviour {

    public bool m_causeDamage = false;



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
        if (m_causeDamage)
        {
            if (GameController.Instance.IsBoss(other.GetComponent<Rigidbody>()))
            {
                GameController.Instance.HitBoss(20f);
                m_causeDamage = false;
            }
        }
    }



}

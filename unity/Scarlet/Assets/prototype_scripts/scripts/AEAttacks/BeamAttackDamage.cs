using UnityEngine;
using System.Collections;

public class BeamAttackDamage : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnTriggerEnter(Collider other)
    {
        HandleScarletCollision(other, 35f);
    }

    void OnTriggerStay(Collider other)
    {
        HandleScarletCollision(other, 35f );
    }

    private void HandleScarletCollision(Collider other, float damage)
    {
        if (other.GetComponent<Rigidbody>() != null &&
            GameController.Instance.IsScarlet(other.GetComponent<Rigidbody>()))
        {
            GameController.Instance.HitScarlet(GameController.Instance.m_Boss, damage, false);
        }
    }
}

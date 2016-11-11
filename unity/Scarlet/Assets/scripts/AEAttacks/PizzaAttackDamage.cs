using UnityEngine;
using System.Collections;

public class PizzaAttackDamage : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        HandleScarletCollision(other, 10f);
    }

    void OnTriggerStay(Collider other)
    {
        HandleScarletCollision(other, 20f * Time.deltaTime);
    }

    private void HandleScarletCollision(Collider other, float damage)
    {
        if (other.GetComponent<Rigidbody>() != null &&
            GameController.Instance.IsScarlet(other.GetComponent<Rigidbody>()))
        {
            GameController.Instance.HitScarlet(damage);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{ 

    void OnTriggerEnter(Collider other)
    {
        HandleScarletCollision(other, 15f);
    }

    void OnTriggerStay(Collider other)
    {
        HandleScarletCollision(other, 15f);
    }

    private void HandleScarletCollision(Collider other, float damage)
    {
        if (other.GetComponent<Rigidbody>() != null &&
            GameController.Instance.IsScarlet(other.GetComponent<Rigidbody>()))
        {
            GameController.Instance.HitScarlet(transform.gameObject, damage, true);
        }
    }
}

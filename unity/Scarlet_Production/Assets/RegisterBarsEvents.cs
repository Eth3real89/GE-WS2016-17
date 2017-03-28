using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterBarsEvents : MonoBehaviour
{
    public bool addForce;

    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        CaveEntrance.explodeEvent += Explode;
        CaveEntrance.dropEvent += Drop;
        CaveEntrance.destroyEvent += Remove;
    }

    private void Explode()
    {
        rb.isKinematic = false;
        if (addForce)
        {
            Vector3 origin = GameObject.FindGameObjectWithTag("Player").transform.position;
            rb.AddExplosionForce(500, origin, 50);
        }
    }

    private void Drop()
    {
        rb.isKinematic = true;
        col.enabled = false;
    }

    private void Remove()
    {
         // don't remove
    }
}

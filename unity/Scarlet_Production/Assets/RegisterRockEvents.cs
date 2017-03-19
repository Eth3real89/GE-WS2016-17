﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterRockEvents : MonoBehaviour
{
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
    }

    private void Drop()
    {
        col.enabled = false;
    }

    private void Remove()
    {
        gameObject.SetActive(false);
    }
}

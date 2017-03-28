using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableColliderDelayed : MonoBehaviour
{
    public float delay;

    void Start()
    {
        StartCoroutine(EnableDelayed());
    }

    IEnumerator EnableDelayed()
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Collider>().enabled = true;
    }

}

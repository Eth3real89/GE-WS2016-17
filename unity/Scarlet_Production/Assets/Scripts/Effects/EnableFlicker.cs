using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableFlicker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.parent.GetComponent<StartFlicker>().InitFlicker();
        RenderSettings.ambientIntensity = 0;
        Camera.main.clearFlags = CameraClearFlags.Color;
        Camera.main.backgroundColor = Color.black;
        Destroy(gameObject);
    }
}

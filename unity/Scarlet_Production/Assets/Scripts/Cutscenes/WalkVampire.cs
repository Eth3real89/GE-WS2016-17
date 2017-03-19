using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkVampire : MonoBehaviour
{
    void Start()
    {
        GetComponent<Animator>().SetFloat("Speed", 1);
        new FARQ().ClipName("vampire").StartTime(42.2f).EndTime(65.4f).Location(Camera.main.transform).Play();
    }
}

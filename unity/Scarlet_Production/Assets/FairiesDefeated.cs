using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairiesDefeated : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.SetInt("FairiesDefeated", 1);
        PlayerPrefs.Save();
    }
}

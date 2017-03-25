using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondEncounter : MonoBehaviour
{
    public void Encounter()
    {
        PlayerPrefs.SetInt("SecondEncounter", 1);
        PlayerPrefs.Save();
    }
}

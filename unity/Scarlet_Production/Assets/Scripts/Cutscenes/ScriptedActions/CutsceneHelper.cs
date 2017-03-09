using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneHelper : MonoBehaviour
{
    public static CutsceneHelper Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelAnimations : MonoBehaviour
{
    public void BanishIdle()
    {
        GetComponent<Animator>().SetTrigger("BanishTrigger");
    }

    public void BanishRitual()
    {
        GetComponent<Animator>().SetTrigger("BanishRitual");
    }
}

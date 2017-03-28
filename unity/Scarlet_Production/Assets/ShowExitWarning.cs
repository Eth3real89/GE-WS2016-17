using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowExitWarning : MonoBehaviour
{
    public string warning;
    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            FindObjectOfType<ShowRewardMessageController>().StartFadeIn(warning);
            hasTriggered = true;
        }
    }
}

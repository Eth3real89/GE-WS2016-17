using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    private float maxLimitLowered = 57;
    private bool soundPlayed;

    public void Raise()
    {
        new FARQ().ClipName("bridge").Location(transform).Play();
        GetComponent<Animation>().Play();
    }

    public void Lower()
    {
        if (!soundPlayed)
        {
            new FARQ().ClipName("bridge").Location(transform).Play();
            soundPlayed = true;
        }
        JointLimits limits = new JointLimits();
        limits.max = maxLimitLowered;
        GetComponent<HingeJoint>().limits = limits;
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsActivator : MonoBehaviour {

    public void ActivateControls()
    {
        PlayerControls controls = FindObjectOfType<PlayerControls>();
        if (controls != null)
            controls.ActivateInitially();
    }

}

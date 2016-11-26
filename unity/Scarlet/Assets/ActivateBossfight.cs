using UnityEngine;
using System.Collections;

public class ActivateBossfight : MonoBehaviour
{
    public PlayerControlsCharController controls;
    public CameraController cc;
    public CameraController2D cc2d;

    public GameObject[] objectsToEnable;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (GameObject o in objectsToEnable)
            {
                o.SetActive(true);
            }
            cc.enabled = true;
            cc2d.enabled = false;
            controls.currentControlMode = PlayerControlsCharController.ControlMode.BossFight;
            Destroy(this.gameObject);
        }
    }
}

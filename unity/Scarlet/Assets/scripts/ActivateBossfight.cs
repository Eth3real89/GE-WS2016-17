using UnityEngine;
using System.Collections;

public class ActivateBossfight : MonoBehaviour
{
    public PlayerControlsCharController controls;
    public CameraController cc;
    public CameraController2D cc2d;

    public GameObject[] objectsToEnable;
    public GameObject[] arenaWalls;
    public float animationTime = 6f;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            controls.m_ControlsEnabled = false;
            controls.CancelMovement();
            foreach (GameObject w in arenaWalls)
            {
                if (w.GetComponent<AudioSource>())
                {
                    w.GetComponent<AudioSource>().Play();
                }
                w.GetComponent<Animation>().Play("walls_go_up");
            }
            StartCoroutine(WallsAreUp());
        }
    }

    private IEnumerator WallsAreUp()
    {
        yield return new WaitForSeconds(animationTime);
        controls.m_ControlsEnabled = true;
        foreach (GameObject o in objectsToEnable)
        {
            o.SetActive(true);
        }
        cc.enabled = true;
        cc2d.enabled = false;
        controls.currentControlMode = PlayerControlsCharController.ControlMode.BossFight;
        Destroy(gameObject);
    }
}
using UnityEngine;
using System.Collections;

public class PlaceCollectible : MonoBehaviour
{
    public bool hasCollectible;
    public GameObject border;
    public GameObject wall;
    public GameObject collectible;
    public Transform newPosition;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (hasCollectible)
            {
                collectible.transform.parent = newPosition;
                collectible.transform.localPosition = Vector3.zero;
                collectible.SetActive(true);
                border.SetActive(false);
                wall.GetComponent<Animation>().Play("walls_go_down");
                wall.GetComponent<AudioSource>().Play();
                GetComponent<AudioSource>().Play();
                hasCollectible = false;
            }
        }
    }
}

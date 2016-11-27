using UnityEngine;
using System.Collections;

public class PlaceCollectible : MonoBehaviour
{
    public bool hasCollectible;
    public Transform newPosition;
    public GameObject collectible;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (hasCollectible)
            {
                GetComponent<AudioSource>().Play();
                collectible.transform.parent = newPosition;
                collectible.transform.localPosition = Vector3.zero;
                collectible.SetActive(true);
                hasCollectible = false;
            }
        }
    }
}

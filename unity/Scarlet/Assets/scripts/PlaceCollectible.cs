using UnityEngine;
using System.Collections;

public class PlaceCollectible : MonoBehaviour
{
    public bool hasCollectible;
    public GameObject placedObject;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (hasCollectible)
            {
                GetComponent<AudioSource>().Play();
                placedObject.SetActive(true);
                hasCollectible = false;
            }
        }
    }
}

using UnityEngine;

public class PickUp : MonoBehaviour {
    public PlaceCollectible pcScript;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            pcScript.hasCollectible = true;
            Destroy(gameObject);
        }
    }
}

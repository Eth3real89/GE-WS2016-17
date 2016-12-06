using UnityEngine;

public class PickUp : MonoBehaviour {
    public PlaceCollectible pcScript;
    public AudioClip pickUpSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(pickUpSound, Camera.main.transform.position);
            pcScript.hasCollectible = true;
            gameObject.SetActive(false);
            Destroy(this);
        }
    }
}

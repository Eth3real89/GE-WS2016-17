using UnityEngine;
using System.Collections;

public class PlaceCollectible : MonoBehaviour
{
    public bool hasCollectible;
    public GameObject hint;
    public GameObject border;
    public GameObject wall;
    public GameObject collectible;
    public Transform newPosition;
    public Font fantasyFont;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (hasCollectible)
            {
                hint.GetComponent<TextMesh>().text = "HansWurscht!Wurscht\nTestNextLine";
                hint.GetComponent<TextMesh>().font = fantasyFont;
                hint.GetComponent<Renderer>().sharedMaterial = fantasyFont.material;
                collectible.transform.parent = newPosition;
                collectible.transform.localPosition = Vector3.zero;
                collectible.SetActive(true);
                border.SetActive(false);
                wall.GetComponent<Animation>().Play("walls_go_down");
                wall.GetComponent<AudioSource>().Play();
                GetComponent<AudioSource>().Play();
                hasCollectible = false;
                hint.SetActive(true);
            }
            else
            {
                hint.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            hint.SetActive(false);
        }
    }
}

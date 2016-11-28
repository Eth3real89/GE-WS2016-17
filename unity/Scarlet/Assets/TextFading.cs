using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TextFading : MonoBehaviour {

    private Text t;
    private bool isBlinking = false;

	// Use this for initialization
	void Start () {
	    t = GetComponent<Text>();

        t.enabled = true;

        StartBlinking();
	}

    public void StartBlinking()
    {
        if(isBlinking)
        {
            return;
        }

        if(t != null)
        {
            isBlinking = true;

            InvokeRepeating("ToggleState", 0.5f, 1f);
        }
    }

    public void ToggleState()
    {
        t.enabled = !t.enabled;
    }

	// Update is called once per frame
	void Update () {
 
	}
}

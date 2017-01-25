using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaEnterTextController : MonoBehaviour {

    public GameObject textHint;
    public int timeShowHint;
    public int delay;

    private bool fadeIn;
    private bool fadeOut = false;
    private float time = 0;

    // Use this for initialization
    void Start () {
        fadeIn = true;
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        if(time > delay && fadeIn)
        {
            StartCoroutine(FadeTo(1.0f, 0.6f));
            fadeIn = false;
            fadeOut = true;
            time = 0;
        }
        if(time > timeShowHint && fadeOut)
        {
            fadeOut = false;
            StartCoroutine(FadeTo(0.0f, 0.6f));
        }
    }
    

    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = 1-aValue;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColorRed = new Color(0.65f, 0, 0, Mathf.Lerp(alpha, aValue, t));
            textHint.GetComponentInChildren<Text>().color = newColorRed;
            textHint.GetComponentInChildren<Image>().color = newColorRed;

            yield return null;
        }
        textHint.GetComponentInChildren<Image>().color = new Color(0.65f, 0, 0, aValue);
        textHint.GetComponentInChildren<Text>().color = new Color(0.65f, 0, 0, aValue);

    }

}

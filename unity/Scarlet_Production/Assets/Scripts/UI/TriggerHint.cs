using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHint : MonoBehaviour {

    public GameObject hint;
    //public GameObject text;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeTo(1.0f, 0.8f));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeTo(0.0f, 0.8f));
        }
    }


    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = hint.GetComponent<SpriteRenderer>().color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            //Color newColorText = new Color(0.706f, 0, 0, Mathf.Lerp(alpha, aValue, t));
            hint.GetComponent<SpriteRenderer>().color = newColor;
            //text.GetComponent<TextMesh>().color = newColorText;
            
            yield return null;
        }
    }
}

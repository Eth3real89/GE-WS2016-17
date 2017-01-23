using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LightFieldPromptController : MonoBehaviour {
    
    public GameObject textHint;
    public string message;
    public GameObject[] FollowingObjects;

    private Text textField;

    // Use this for initialization
    void Start()
    {
        //var lightType = GetComponent<LightField>().m_Class;
        //Debug.Log(lightType);

        textField = textHint.GetComponentInChildren<Text>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textField.text = message;
            ActivateFollowingObjects();

            StartCoroutine(FadeTo(1.0f, 0.6f));
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeTo(0.0f, 0.6f));
        }
    }


    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = textField.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColorRed = new Color(0.65f, 0, 0, Mathf.Lerp(alpha, aValue, t));
            textField.color = newColorRed;

            yield return null;
        }
        textField.color = new Color(0.65f, 0, 0, aValue);
    }
    private void ActivateFollowingObjects()
    {
        foreach(GameObject obj in FollowingObjects)
        {
            obj.SetActive(true);
        }
    }
}

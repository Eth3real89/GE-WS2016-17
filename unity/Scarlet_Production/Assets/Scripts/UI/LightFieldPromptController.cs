using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LightFieldPromptController : MonoBehaviour {
    
    public GameObject textHint;
    public string message;
    public GameObject[] FollowingObjects;

    private Text textField;
    private Image background;

    // Use this for initialization
    void Start()
    {
        //var lightType = GetComponent<LightField>().m_Class;
        //Debug.Log(lightType);

        textField = textHint.GetComponentInChildren<Text>();
        background = textHint.GetComponentInChildren<Image>();
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
        float alphaBlack = background.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColorRed = new Color(0.65f, 0, 0, Mathf.Lerp(alpha, aValue, t));
            Color newColorBlack = new Color(0, 0, 0, Mathf.Lerp(alphaBlack, aValue / 1.5f, t));
            textField.color = newColorRed;
            background.color = newColorBlack;
            yield return null;
        }
        textField.color = new Color(0.65f, 0, 0, aValue);
        background.color = new Color(0, 0, 0, aValue / 1.5f);
    }

    private void ActivateFollowingObjects()
    {
        foreach(GameObject obj in FollowingObjects)
        {
            obj.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemPickupController : MonoBehaviour
{

    public GameObject textHint;
    public bool charging;
    public bool m_ShowReward = false;
    public string m_TextForReward = "";

    private bool m_StopInteraction = false;
    private Text textField;
    private Image[] images;
    private Interactor interactor;

    void Start()
    {
        images = textHint.GetComponentsInChildren<Image>();
        textField = textHint.GetComponentInChildren<Text>();
        interactor = transform.parent.GetComponent<Interactor>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!m_StopInteraction && other.CompareTag("Player"))
        {
            StartCoroutine(FadeTo(1.0f, 0.6f));
            textHint.GetComponentInChildren<ButtonPromptController>().IsInTriggerArea(gameObject, true);
            other.GetComponentInChildren<PlayerInteractionCommand>().m_CurrentInteractor = interactor;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeTo(0.0f, 0.6f));
            textHint.GetComponentInChildren<ButtonPromptController>().IsInTriggerArea(gameObject, false);
            other.GetComponentInChildren<PlayerInteractionCommand>().m_CurrentInteractor = null;
        }
    }

    public void Interacting(bool isInteracting)
    {
        if (!m_StopInteraction)
        {
            if (isInteracting)
            {
                images[1].color = new Color(0.65f, 0, 0, 0);
                StartCoroutine(FadeTo(0.0f, 0.4f));
            }
            else
            {
                images[1].color = new Color(0.65f, 0, 0, 1);
                StartCoroutine(FadeTo(1.0f, 0.4f));
            }
        }
    }

    public void StopInteraction()
    {
        m_StopInteraction = true;
        OnTriggerExit(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>());
        images[1].color = new Color(0.65f, 0, 0, 0);
        StartCoroutine(FadeTo(0.0f, 0.4f));
    }

    public void OnItemPickedUp()
    {
        //Fade doesn't work because Gameobject is destroyed/invisible
        if (charging)
        {
            images[0].color = new Color(0, 0, 0, 0);
            images[1].color = new Color(0.65f, 0, 0, 0);
            images[2].color = new Color(0.65f, 0, 0, 0);
        }
        else
        {
            images[0].color = new Color(0, 0, 0, 0);
            images[1].color = new Color(0.65f, 0, 0, 0);
        }
        textField.color = new Color(0.65f, 0, 0, 0);
        textHint.GetComponentInChildren<ButtonPromptController>().enabled = false;
        if (m_ShowReward)
        {
            FindObjectOfType<ShowRewardMessageController>().StartFadeIn(m_TextForReward);
        }
    }

    public void UpdatePickup(float floatcurrentTime)
    {
        images[1].fillAmount = Mathf.Lerp(0.0f, 1.0f, floatcurrentTime);
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = textField.color.a;
        float alphaBlack = images[0].color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColorRed = new Color(0.65f, 0, 0, Mathf.Lerp(alpha, aValue, t));
            Color newColorBlack = new Color(0, 0, 0, Mathf.Lerp(alphaBlack, aValue, t));
            if (charging)
            {
                images[0].color = newColorBlack;
                images[2].color = newColorRed;
            }
            else
            {
                images[0].color = newColorBlack;
                images[1].color = newColorRed;
            }
            textField.color = newColorRed;

            yield return null;
        }
        if (charging)
        {
            images[0].color = new Color(0, 0, 0, aValue);
            images[2].color = new Color(0.65f, 0, 0, aValue);
        }
        else
        {
            images[0].color = new Color(0, 0, 0, aValue);
            images[1].color = new Color(0.65f, 0, 0, aValue);
        }
        textField.color = new Color(0.65f, 0, 0, aValue);
    }
}

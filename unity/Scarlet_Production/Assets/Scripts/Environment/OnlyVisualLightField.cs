using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyVisualLightField : MonoBehaviour {

    private static List<OnlyVisualLightField> s_ActiveFields;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerManager>() != null)
            AddToActiveFields();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerManager>() != null)
            RemoveFromActiveFields();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerManager>() != null)
            AddToActiveFields();
    }

    private void AddToActiveFields()
    {
        if (s_ActiveFields == null)
            s_ActiveFields = new List<OnlyVisualLightField>();

        int prevCount = s_ActiveFields.Count;

        if (!s_ActiveFields.Contains(this))
            s_ActiveFields.Add(this);

        if (s_ActiveFields.Count != 0 && prevCount == 0)
        {
            EffectController.Instance.EnterStrongLight();
        }
    }

    private void RemoveFromActiveFields()
    {
        if (s_ActiveFields == null)
            s_ActiveFields = new List<OnlyVisualLightField>();

        if (s_ActiveFields.Contains(this))
            s_ActiveFields.Remove(this);

        if (s_ActiveFields.Count == 0)
            EffectController.Instance.ExitStrongLight();
    }

}

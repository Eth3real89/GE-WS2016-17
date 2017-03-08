using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyVisualLightField : MonoBehaviour {

    private static List<OnlyVisualLightField> s_ActiveFields;
    private float m_Multiplier = 4f;

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

    /*private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerManager>() != null)
            AddToActiveFields();
    }*/

    private void AddToActiveFields()
    {
        if (s_ActiveFields == null)
            s_ActiveFields = new List<OnlyVisualLightField>();

        int prevCount = s_ActiveFields.Count;

        if (!s_ActiveFields.Contains(this))
            s_ActiveFields.Add(this);

        EffectController.Instance.MakeLightBloom(s_ActiveFields.Count * m_Multiplier, s_ActiveFields.Count * m_Multiplier);
    }

    private void RemoveFromActiveFields()
    {
        if (s_ActiveFields == null)
            s_ActiveFields = new List<OnlyVisualLightField>();

        if (s_ActiveFields.Contains(this))
            s_ActiveFields.Remove(this);

        EffectController.Instance.MakeLightBloom(s_ActiveFields.Count * m_Multiplier, s_ActiveFields.Count * m_Multiplier);
    }

    internal static void ResetFields()
    {
        s_ActiveFields = new List<OnlyVisualLightField>();
    }
}

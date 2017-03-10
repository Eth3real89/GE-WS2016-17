using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactor : MonoBehaviour
{
    public string m_InteractionCueSound;
    public bool m_UseAnimation;

    public virtual void Interact()
    {
        if (m_InteractionCueSound != null && m_InteractionCueSound != "")
        {
            AudioController.Instance.PlaySound(m_InteractionCueSound);
        }
    }
}
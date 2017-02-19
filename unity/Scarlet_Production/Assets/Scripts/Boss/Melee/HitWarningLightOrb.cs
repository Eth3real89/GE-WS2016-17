using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitWarningLightOrb : HitWarning
{
    public GameObject[] m_HitWarnings;

    public override void ShowWarning(int attackAnimation)
    {
        SetOrbVisibility(attackAnimation, true);
    }

    public override void HideWarning(int attackAnimation)
    {
        SetOrbVisibility(attackAnimation, false);
    }

    protected virtual void SetOrbVisibility(int attackAnimation, bool active)
    {
        if (m_HitWarnings != null && m_HitWarnings.Length > attackAnimation && m_HitWarnings[attackAnimation] != null)
        {
            m_HitWarnings[attackAnimation].SetActive(active);
        }
    }
}

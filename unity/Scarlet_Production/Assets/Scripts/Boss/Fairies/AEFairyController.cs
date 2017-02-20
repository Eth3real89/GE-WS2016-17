﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEFairyController : FairyController
{

    public GameObject m_LightGuardContainer;
    public LightGuard m_LightGuard;

    public override void Initialize(FairyControllerCallbacks callbacks)
    {
        base.Initialize(callbacks);
        m_LightGuard = m_LightGuardContainer.GetComponentInChildren<LightGuard>();

        m_NotDeactivated = true;
    }

    public virtual void ExpandLightGuard()
    {
        if (!m_LightGuard.gameObject.activeSelf)
        {
            m_LightGuardContainer.SetActive(true);
            if (m_LightGuard != null)
                m_LightGuard.Enable();
        }
    }

    public virtual void DisableLightGuard()
    {
        m_LightGuardContainer.SetActive(false);
    }
       
}

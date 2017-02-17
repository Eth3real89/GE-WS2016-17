using System.Collections;
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
    }

    public virtual void ExpandLightGuard()
    {
        m_LightGuardContainer.SetActive(true);
        if (m_LightGuard != null)
            m_LightGuard.Enable();
    }

    public virtual void DisableLightGuard()
    {
        m_LightGuardContainer.SetActive(false);
    }
       
}

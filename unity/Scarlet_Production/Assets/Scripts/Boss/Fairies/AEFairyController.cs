using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEFairyController : FairyController
{

    public GameObject m_LightGuardContainer;
    public LightGuard m_LightGuard;

    public float m_RotationTime = 25f;

    public override void Initialize(FairyControllerCallbacks callbacks)
    {
        base.Initialize(callbacks);
        m_LightGuard = m_LightGuardContainer.GetComponentInChildren<LightGuard>();

        for (int i = 0; i < m_Combos.Length; i++)
        {
            AttackCombo combo = m_Combos[i];
            for (int j = 0; j < combo.m_Attacks.Length; j++)
            {
                if (combo.m_Attacks[j] is GrowingThenRotatingConeAttack)
                {
                    ((GrowingThenRotatingConeAttack)combo.m_Attacks[j]).m_RotationTime = m_RotationTime;
                }
            }
        }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorFairyController : BossController {

    public FairyControllerCallbacks m_Callback;

    public void Initialize(FairyControllerCallbacks callbacks)
    {
        RegisterComboCallback();
        m_BossHittable.RegisterInterject(this);
        m_Callback = callbacks;
    }

    public void StartCombo(int index)
    {
        m_Combos[index].LaunchCombo();
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        m_ActiveCombo = null;

        m_BossHittable.RegisterInterject(this);
        m_BlockingBehaviour.m_TimesBlockBeforeParry = m_MaxBlocksBeforeParry;

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        m_Callback.OnComboEnd(this);
    }
}

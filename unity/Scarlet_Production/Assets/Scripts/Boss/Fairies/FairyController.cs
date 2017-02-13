using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyController : BossController {

    public FairyControllerCallbacks m_Callback;

    public virtual void Initialize(FairyControllerCallbacks callbacks)
    {
        RegisterComboCallback();
        m_BossHittable.RegisterInterject(this);
        m_Callback = callbacks;
    }

    public virtual void StartCombo(int index)
    {
        m_Combos[index].LaunchCombo();
    }

    public override void OnComboEnd(AttackCombo combo)
    {
        MLog.Log(LogType.FairyLog, 1, "Combo End, Armor Fairy Controller, " + combo);

        m_ActiveCombo = null;

        m_BossHittable.RegisterInterject(this);
        m_BlockingBehaviour.m_TimesBlockBeforeParry = m_MaxBlocksBeforeParry;

        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        m_Callback.OnComboEnd(this);
    }

    public virtual void Continue()
    {
        if (m_NextComboTimer != null)
            StopCoroutine(m_NextComboTimer);

        m_NextComboTimer = StartNextComboAfter(m_Combos[m_CurrentComboIndex].m_TimeAfterCombo);
        StartCoroutine(m_NextComboTimer);
    }

}

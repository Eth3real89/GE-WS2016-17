using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyBossfightPhase : MonoBehaviour, FairyControllerCallbacks {

    public AEFairyController m_AEFairyController;
    public ArmorFairyController m_ArmorFairyController;

    protected bool m_Active = false;
    protected FairyPhaseCallbacks m_Callback;

    public virtual void StartPhase(FairyPhaseCallbacks callbacks)
    {
        m_Active = true;
        m_Callback = callbacks;

        m_AEFairyController.Initialize(this);
        m_ArmorFairyController.Initialize(this);

        m_Callback.OnPhaseStart(this);

        StartCombo();
    }

    protected virtual void Update()
    {
        if (!m_Active)
            return;
    }

    protected virtual void EndPhase()
    {
        m_Callback.OnPhaseEnd(this);

        if (m_AEFairyController != null)
            m_AEFairyController.m_NotDeactivated = false;

        if (m_ArmorFairyController != null)
            m_ArmorFairyController.m_NotDeactivated = false;
    }

    public virtual void StartCombo()
    {
        m_AEFairyController.StartCombo(0);
        m_ArmorFairyController.StartCombo(0);
    }

    public void OnComboStart(FairyController controller)
    {
        MLog.Log(LogType.FairyLog, 0, "OnComboStart, Phase, " + controller);
    }

    public virtual void OnComboEnd(FairyController controller)
    {
        MLog.Log(LogType.FairyLog, 0, "OnComboEnd, Phase, " + controller);

        if (m_Active)
           controller.Continue();
    }
    
    public virtual void EndCombo()
    {
        m_AEFairyController.CancelComboIfActive();
        m_ArmorFairyController.CancelComboIfActive();
    }

}

public interface FairyControllerCallbacks
{
    void OnComboStart(FairyController controller);
    void OnComboEnd(FairyController controller);
}

public interface FairyPhaseCallbacks
{
    void OnPhaseStart(FairyBossfightPhase phase);
    void OnPhaseEnd(FairyBossfightPhase phase);
}

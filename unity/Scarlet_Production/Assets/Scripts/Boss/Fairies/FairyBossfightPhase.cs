using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyBossfightPhase : MonoBehaviour, FairyControllerCallbacks {

    public AEFairyController m_AEFairyController;
    public ArmorFairyController m_ArmorFairyController;

    public virtual void StartPhase(FairyBossfight bossfight)
    {
        m_AEFairyController.Initialize(this);
        m_ArmorFairyController.Initialize(this);

        StartCombo();
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

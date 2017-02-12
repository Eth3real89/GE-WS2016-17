using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyBossfightPhase : MonoBehaviour, FairyControllerCallbacks {

    public AEFairyController m_AEFairyController;
    public ArmorFairyController m_ArmorFairyController;

    private IEnumerator m_ComboEndEnumerator;

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

    public void OnComboStart(BossController controller)
    {

    }

    public virtual void OnComboEnd(BossController controller)
    {
        MLog.Log(LogType.FairyLog, 0, "OnComboEnd, Phase, " + controller);

        if (m_ComboEndEnumerator != null)
        {
            StopCoroutine(m_ComboEndEnumerator);
            StartCombo();
        }
        else
        {
            m_ComboEndEnumerator = WaitForOtherComboToEnd();
            StartCoroutine(m_ComboEndEnumerator);
        }
    }

    private IEnumerator WaitForOtherComboToEnd()
    {
        while (true)
            yield return null;
    }

    public virtual void EndCombo()
    {
        m_AEFairyController.CancelComboIfActive();
        m_ArmorFairyController.CancelComboIfActive();
    }

}

public interface FairyControllerCallbacks
{
    void OnComboStart(BossController controller);
    void OnComboEnd(BossController controller);
}

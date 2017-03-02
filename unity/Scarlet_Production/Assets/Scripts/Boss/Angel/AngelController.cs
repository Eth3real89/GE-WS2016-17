using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelController : BossController {

    public AngelWeapons m_Weapons;

    public BossfightCallbacks m_Callback;
    public int m_StartAttackIndex = 0;

    public virtual void StartPhase(BossfightCallbacks callback)
    {
        //Time.timeScale = 0.5f;

        MLog.Log(LogType.AngelLog, "Starting Phase, Angel, " + this);
        this.m_Callback = callback;

        RegisterComboCallback();
        m_BossHittable.RegisterInterject(this);

        StartFirstCombo();
    }

    protected virtual void StartFirstCombo()
    {
        m_CurrentComboIndex = m_StartAttackIndex - 1;
        StartCoroutine(StartNextComboAfter(0.5f));
    }

    protected override IEnumerator StartNextComboAfter(float time)
    {
        yield return new WaitForSeconds(time);

        int nextCombo = m_CurrentComboIndex + 1 >= m_Combos.Length ? 0 : m_CurrentComboIndex + 1;
        m_Weapons.ChangeTipTo(((AngelCombo)m_Combos[nextCombo]).m_AssociatedTip, OnTipChanged(), this);

    }

    protected virtual IEnumerator OnTipChanged()
    {
        yield return null;
        StartNextCombo();
    }


}

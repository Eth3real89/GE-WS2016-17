using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Workaround...
public class AngelOnlyAnimationAttack : AngelAttack {

    public float m_AnimTime;
    public string m_AnimName;

    protected IEnumerator m_Timer;

    protected bool m_Cancelled = false;

    public override void StartAttack()
    {
        m_Cancelled = false;
        base.StartAttack();
        m_Animator.SetTrigger(m_AnimName);

        m_Timer = WaitUntilEnd();
        StartCoroutine(m_Timer);

        MLog.Log(LogType.AngelLog, 1, "Angel Only Animation Attack, StartAttack, " + this);
    }

    protected virtual IEnumerator WaitUntilEnd()
    {
        yield return new WaitForSeconds(AdjustTime(m_AnimTime));
        MLog.Log(LogType.AngelLog, 1, "Angel Only Animation Attack, Finished Waiting, " + m_Cancelled + " " + this);

        if (!m_Cancelled)
            m_Callback.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        MLog.Log(LogType.AngelLog, 1, "Angel Only Animation Attack, Cancelled, " + this);

        if (m_Timer != null)
            StopCoroutine(m_Timer);

        m_Cancelled = true;
    }
}

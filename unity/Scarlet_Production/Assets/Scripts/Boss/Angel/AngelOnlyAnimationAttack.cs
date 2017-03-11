using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Workaround...
public class AngelOnlyAnimationAttack : BossAttack {

    public float m_AnimTime;
    public string m_AnimName;

    protected IEnumerator m_Timer;

    public override void StartAttack()
    {
        base.StartAttack();
        m_Animator.SetTrigger(m_AnimName);

        m_Timer = WaitUntilEnd();
        StartCoroutine(m_Timer);
    }

    protected IEnumerator WaitUntilEnd()
    {
        yield return new WaitForSeconds(m_AnimTime);

        print("???");
        m_Callback.OnAttackEnd(this);
    }

    public override void CancelAttack()
    {
        if (m_Timer != null)
            StopCoroutine(m_Timer);
    }

}

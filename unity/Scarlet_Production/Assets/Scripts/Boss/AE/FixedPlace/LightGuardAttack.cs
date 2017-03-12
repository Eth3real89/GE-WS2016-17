using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGuardAttack : AEAttack {

    public LightGuard m_LightGuard;

    public float m_Time;
    protected IEnumerator m_Timer;

    public override void StartAttack()
    {
        base.StartAttack();

        m_LightGuard.gameObject.SetActive(true);
        m_LightGuard.DetachVisualsFromParent();
        m_LightGuard.Enable();

        m_Timer = Timer();
        StartCoroutine(m_Timer);

        m_Callback.OnAttackEnd(this);
    }

    protected virtual IEnumerator Timer()
    {
        yield return new WaitForSeconds(m_Time);
        m_LightGuard.Disable();
        m_LightGuard.ReattachVisualsToParent();
    }

    public override void CancelAttack()
    {
        base.CancelAttack();

        if (m_Timer != null)
        {
            StopCoroutine(m_Timer);
            m_Timer = null;
        }

        if (m_LightGuard.gameObject.activeSelf)
        {
            m_LightGuard.Disable();
            m_LightGuard.ReattachVisualsToParent();
        }
    }

    public bool IsActive()
    {
        return m_Timer != null;
    }

}

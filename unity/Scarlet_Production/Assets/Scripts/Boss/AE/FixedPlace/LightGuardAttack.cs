﻿using System.Collections;
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
        m_LightGuard.ReattachVisualsToParent();
        m_LightGuard.gameObject.SetActive(false);
    }

    public override void CancelAttack()
    {
        base.CancelAttack();
        if (m_Timer != null)
            StopCoroutine(m_Timer);

        m_LightGuard.ReattachVisualsToParent();
        m_LightGuard.gameObject.SetActive(false);
    }

    public bool IsActive()
    {
        return m_Timer != null;
    }

}

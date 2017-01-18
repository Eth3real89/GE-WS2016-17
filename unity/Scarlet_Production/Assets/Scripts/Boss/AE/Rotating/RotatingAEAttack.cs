using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingAEAttack : AEAttack, ExpandingAEDamage.ExpandingDamageCallbacks
{
    public float m_ExpandTime = 0.3f;
    public float m_ExpandScale = 8;

    public float m_RotationTime = 3;
    public float m_RotationAngle = 90;

    public ExpandingAEDamage m_Damage;

    public override void StartAttack()
    {
        base.StartAttack();

        m_Damage.SetAngle(- m_RotationAngle / 2);
        m_Damage.Expand(m_ExpandTime, m_ExpandScale, this);
    }

    public void OnExpansionOver()
    {
        m_Damage.Rotate(m_RotationTime, m_RotationAngle, this);
    }

    public void OnRotationOver()
    {
        StartCoroutine(RemoveBeamAfterWaiting());
    }

    private IEnumerator RemoveBeamAfterWaiting()
    {
        yield return new WaitForSeconds(0.5f);

        m_Damage.gameObject.SetActive(false);
        m_Damage.m_Active = false;
        m_Callback.OnAttackEnd(this);
    }
}

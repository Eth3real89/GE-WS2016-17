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

    public TurnTowardsScarlet m_InitialTurn;
    public float m_InitialTurnTrackSpeed = 45;
    public float m_TurnTime = 2f;
    private float m_PrevTurnSpeed;

    private IEnumerator m_ExpansionEnumerator;

    public override void StartAttack()
    {
        base.StartAttack();

        m_PrevTurnSpeed = m_InitialTurn.m_TurnSpeed;
        m_InitialTurn.m_TurnSpeed = 9999;
        m_InitialTurn.DoTurn();
        m_InitialTurn.m_TurnSpeed = m_InitialTurnTrackSpeed;

        m_ExpansionEnumerator = BeforeExpansion();
        StartCoroutine(m_ExpansionEnumerator);
    }

    private IEnumerator BeforeExpansion()
    {
        float t = 0;

        while ((t += Time.deltaTime) < m_TurnTime)
        {
            m_InitialTurn.DoTurn();
            yield return null;
        }

        m_Damage.SetAngle(-m_RotationAngle / 2);
        m_Damage.Expand(m_ExpandTime, m_ExpandScale, this);
    }

    public void OnExpansionOver()
    {
        m_Damage.Rotate(m_RotationTime, m_RotationAngle, this);
    }

    public void OnRotationOver()
    {
        m_ExpansionEnumerator = RemoveBeamAfterWaiting();
        StartCoroutine(m_ExpansionEnumerator);
    }

    public override void CancelAttack()
    {
        base.CancelAttack();

        if (m_ExpansionEnumerator != null)
            StopCoroutine(m_ExpansionEnumerator);

        m_Damage.CancelDamage();
    }

    private IEnumerator RemoveBeamAfterWaiting()
    {
        yield return new WaitForSeconds(0.5f);

        m_Damage.gameObject.SetActive(false);
        m_Damage.m_Active = false;
        m_Callback.OnAttackEnd(this);
    }
}

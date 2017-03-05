using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBeamAEAttack : AEAttack, GroundBeamAEDamage.GroundBeamCallbacks
{
    public float m_ExpandTime = 0.3f;
    public float m_ExpandScale = 3;

    public float m_WaitTimeBetweenExpansions = 1f;

    public float m_SecondExpandTime = 2f;
    public float m_SecondExpandScale = 8;

    public GroundBeamAEDamage m_Damage;

    public TurnTowardsScarlet m_InitialTurn;
    public float m_InitialTurnTrackSpeed = 45;
    public float m_TurnTime = 2f;
    private float m_PrevTurnSpeed;

    private IEnumerator m_ExpansionEnumerator;

    public override void StartAttack()
    {
        base.StartAttack();

        EventManager.TriggerEvent(BeamAEAttack.START_EVENT_NAME);

        m_PrevTurnSpeed = m_InitialTurn.m_TurnSpeed;
        m_InitialTurn.m_TurnSpeed = 9999;
        m_InitialTurn.DoTurn();
        m_InitialTurn.m_TurnSpeed = m_InitialTurnTrackSpeed;

        m_ExpansionEnumerator = BeforeExpansion();
        StartCoroutine(m_ExpansionEnumerator);
    }

    protected virtual IEnumerator BeforeExpansion()
    {
        float t = 0;

        while ((t += Time.deltaTime) < m_TurnTime)
        {
            m_InitialTurn.DoTurn();
            yield return null;
        }
        m_InitialTurn.m_TurnSpeed = m_PrevTurnSpeed;

        m_Damage.gameObject.SetActive(true);
        m_Damage.SetAngle(0);
        m_Damage.Expand(m_ExpandTime, m_ExpandScale, this);

        CameraController.Instance.ZoomOut();
    }

    public virtual void OnExpansionOver()
    {
        m_ExpansionEnumerator = WaitBetweenExpansions();
        StartCoroutine(m_ExpansionEnumerator);
    }

    private IEnumerator WaitBetweenExpansions()
    {
        yield return new WaitForSeconds(m_WaitTimeBetweenExpansions);
        m_Damage.SecondExpansion(m_SecondExpandTime, m_SecondExpandScale, this);
    }

    public void OnSecondExpansionOver()
    {
        m_ExpansionEnumerator = RemoveBeamAfterWaiting();
        StartCoroutine(m_ExpansionEnumerator);
        CameraController.Instance.ActivateDefaultCamera();
    }

    public override void CancelAttack()
    {
        base.CancelAttack();
        CameraController.Instance.ActivateDefaultCamera();

        if (m_ExpansionEnumerator != null)
            StopCoroutine(m_ExpansionEnumerator);

        m_Damage.CancelDamage();
        m_Damage.gameObject.SetActive(false);
    }

    protected virtual IEnumerator RemoveBeamAfterWaiting()
    {
        yield return new WaitForSeconds(0.5f);
        EventManager.TriggerEvent(BeamAEAttack.END_EVENT_NAME);

        m_Damage.gameObject.SetActive(false);
        m_Damage.m_Active = false;
        m_Callback.OnAttackEnd(this);
    }
}

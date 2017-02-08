using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAEAttack : AEAttack, BeamAEDamage.ExpandingDamageCallbacks
{
    public static string START_EVENT_NAME = "beam_attack_start";
    public static string END_EVENT_NAME = "beam_attack_end";

    public float m_ExpandTime = 0.3f;
    public float m_ExpandScale = 8;

    public float m_RotationTime = 3;
    public float m_RotationAngle = 90;

    public BeamAEDamage m_Damage;

    public TurnTowardsScarlet m_InitialTurn;
    public float m_InitialTurnTrackSpeed = 45;
    public float m_TurnTime = 2f;

    public bool m_InitiallyAimAtScarlet = false;

    private float m_PrevTurnSpeed;

    private IEnumerator m_ExpansionEnumerator;

    public override void StartAttack()
    {
        base.StartAttack();

        m_PrevTurnSpeed = m_InitialTurn.m_TurnSpeed;
        m_InitialTurn.m_TurnSpeed = 9999;
        m_InitialTurn.DoTurn();
        m_InitialTurn.m_TurnSpeed = m_InitialTurnTrackSpeed;

        EventManager.TriggerEvent(START_EVENT_NAME);

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

        if (!m_InitiallyAimAtScarlet)
        {
            m_Damage.SetAngle(-m_RotationAngle / 2);
        }
        m_Damage.Expand(m_ExpandTime, m_ExpandScale, this);

        CameraController.Instance.ZoomOut();
    }

    public virtual void OnExpansionOver(BeamAEDamage dmg)
    {
        m_Damage.Rotate(m_RotationTime, m_RotationAngle, this);
    }

    public virtual void OnRotationOver(BeamAEDamage dmg)
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
    }

    protected virtual IEnumerator RemoveBeamAfterWaiting()
    {
        yield return new WaitForSeconds(0.5f);

        EventManager.TriggerEvent(END_EVENT_NAME);

        m_Damage.gameObject.SetActive(false);
        m_Damage.m_Active = false;
        m_Callback.OnAttackEnd(this);
        HideLightGuard();
    }

    public virtual void OnRotation(BeamAEDamage damage, float angle)
    {
        m_Boss.transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}

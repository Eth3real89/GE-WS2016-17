using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAEAttack : AEAttack, BeamAEDamage.ExpandingDamageCallbacks, Damage.DamageCallback
{
    public static string START_EVENT_NAME = "beam_attack_start";
    public static string END_EVENT_NAME = "beam_attack_end";

    public float m_ExpandTime = 0.3f;
    public float m_ExpandScale = 8;

    public float m_RotationTime = 3;
    public float m_RotationAngle = 90;

    public Transform m_Container;
    protected BeamAEDamage m_Damage;

    public TurnTowardsScarlet m_InitialTurn;
    public float m_InitialTurnTrackSpeed = 45;
    public float m_TurnTime = 2f;

    public bool m_InitiallyAimAtScarlet = false;
    public bool m_DoNotTurnBoss = false;

    public bool m_PerfectTrackingAtStart = true;

    public bool m_OverrideDefaultDamage = false;
    public float m_DamageAmount = 40f;

    public bool m_AdjustCamera = true;

    private float m_PrevTurnSpeed;

    private IEnumerator m_ExpansionEnumerator;

    public FARQ m_Audio;

    public override void StartAttack()
    {
        base.StartAttack();

        LoadPrefab();
        SetDamage();

        if (m_PerfectTrackingAtStart)
        {
            m_PrevTurnSpeed = m_InitialTurn.m_TurnSpeed;
            m_InitialTurn.m_TurnSpeed = 9999;
            m_InitialTurn.DoTurn();
            m_InitialTurn.m_TurnSpeed = m_InitialTurnTrackSpeed;
        }

        EventManager.TriggerEvent(START_EVENT_NAME);

        m_ExpansionEnumerator = BeforeExpansion();
        StartCoroutine(m_ExpansionEnumerator);
    }

    protected virtual void SetDamage()
    {
        if (m_OverrideDefaultDamage)
        {
            m_Damage.m_DamageAmount = this.m_DamageAmount;
        }
    }

    protected virtual void LoadPrefab()
    {
        m_Damage = AEPrefabManager.Instance.m_RotatingBeamWrapper.GetComponent<BeamAEDamage>();
        m_Damage.transform.parent = m_Container;
        m_Damage.transform.localPosition = new Vector3(0, 0, 0);
        m_Damage.transform.localRotation = Quaternion.Euler(0, 0, 0);
        m_Damage.transform.localScale = new Vector3(1, 1, 1);
        m_Damage.m_Callback = this;
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
        m_Damage.gameObject.SetActive(true);
        m_Damage.Expand(m_ExpandTime, m_ExpandScale, this);

        FancyAudioEffectsSoundPlayer.Instance.PlayBeamStartSound(m_Damage.transform);

        if (m_AdjustCamera)
            CameraController.Instance.ZoomOut();
    }

    public virtual void OnExpansionOver(BeamAEDamage dmg)
    {
        m_Damage.Rotate(m_RotationTime, m_RotationAngle, this);
        m_Audio = FancyAudioEffectsSoundPlayer.Instance.PlayBeamLoopSound(m_Damage.transform);
    }

    public virtual void OnRotationOver(BeamAEDamage dmg)
    {
        m_ExpansionEnumerator = RemoveBeamAfterWaiting();
        StartCoroutine(m_ExpansionEnumerator);
        if (m_AdjustCamera)
            CameraController.Instance.ActivateDefaultCamera();
    }

    public override void CancelAttack()
    {
        base.CancelAttack();
        if (m_AdjustCamera)
            CameraController.Instance.ActivateDefaultCamera();

        if (m_ExpansionEnumerator != null)
            StopCoroutine(m_ExpansionEnumerator);

        if (m_Audio != null)
            m_Audio.StopIfPlaying();

        if (m_Damage != null)
        {
            m_Damage.CancelDamage();
            m_Damage.gameObject.SetActive(false);
        }
    }

    protected virtual IEnumerator RemoveBeamAfterWaiting()
    {
        m_Audio.StopIfPlaying();
        FancyAudioEffectsSoundPlayer.Instance.PlayBeamEndSound(m_Boss.transform);

        yield return new WaitForSeconds(0.5f);

        EventManager.TriggerEvent(END_EVENT_NAME);

        m_Damage.CancelDamage();
        m_Damage.gameObject.SetActive(false);
        m_Callback.OnAttackEnd(this);
    }

    public virtual void OnRotation(BeamAEDamage damage, float angle)
    {
        if (!m_DoNotTurnBoss)
            m_Boss.transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public void OnParryDamage()
    {
    }

    public void OnBlockDamage()
    {
    }

    public void OnSuccessfulHit()
    {
        PlayerStaggerCommand.StaggerScarletAwayFrom(m_Container.position, 2, true);
    }
}

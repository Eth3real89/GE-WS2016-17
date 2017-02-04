using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashCommand : PlayerCommand, HitInterject
{
    public static string COMMAND_EVENT_TRIGGER = "player_trigger_dash";

    public float m_DashDistance = 4.5f;
    public float m_DashTime = 0.05f;
    public float m_DashDelay = 0.15f;

    public GameObject m_TrailContainer;
    public GameObject m_RendererContainer;

    public AudioSource m_DashAudio;

    public PlayerHittable m_Hittable;
    private HitInterject m_PrevInterject;

    private TrailRenderer m_TrailRenderer;
    private Renderer m_ScarletRenderer;

    private Rigidbody m_ScarletBody;

    private void Start()
    {
        m_CommandName = "Dash";

        if (m_TrailContainer != null)
            m_TrailRenderer = m_TrailContainer.GetComponent<TrailRenderer>();

        if (m_RendererContainer != null)
            m_ScarletRenderer = m_RendererContainer.GetComponentInChildren<Renderer>();

    }

    public override void InitTrigger()
    {
        m_CommandName = "Dash";
        m_Trigger = new PressAxisTrigger(this, m_CommandName);
        m_ScarletBody = m_Scarlet.GetComponent<Rigidbody>();
    }

    public override void TriggerCommand()
    {
        DoDash();
        if (m_Hittable != null)
        {
            m_PrevInterject = m_Hittable.GetInterject();
            m_Hittable.RegisterInterject(this);
        }

        EventManager.TriggerEvent(COMMAND_EVENT_TRIGGER);
    }

    private void DoDash()
    {
        m_Callback.OnCommandStart(m_CommandName, this);
        m_ScarletBody.velocity = new Vector3(0, 0, 0);

        Vector3 dashStart = m_ScarletBody.transform.position;
        Vector3 dashTarget = m_ScarletBody.transform.position + m_ScarletBody.transform.forward * m_DashDistance;

        StartCoroutine(Blink(dashStart, dashTarget));

        if (m_DashAudio != null)
            m_DashAudio.Play();
    }

    private IEnumerator Blink(Vector3 dashStart, Vector3 dashTarget)
    {
        float t = 0;
        OnDashStart();

        while (t < m_DashTime)
        {
            yield return null;

            m_ScarletBody.MovePosition(Vector3.Lerp(dashStart, dashTarget, t / m_DashTime));
            t += Time.deltaTime;
        }

        OnDashEnd();
        m_Callback.OnCommandEnd(m_CommandName, this);
        if (m_Hittable != null)
            m_Hittable.RegisterInterject(m_PrevInterject);
    }

    private void OnDashStart()
    {
        // @todo make invincible

        if (m_TrailRenderer != null)
        {
            m_TrailRenderer.Clear();
            m_TrailRenderer.time = 2;
        }

        if (m_ScarletRenderer != null)
        {
            m_ScarletRenderer.enabled = false;
        }
    }

    private void OnDashEnd()
    {
        // @todo stop invincibility

        if (m_TrailRenderer != null)
        {
            m_TrailRenderer.time = 0;
            m_TrailRenderer.Clear();
        }

        if (m_ScarletRenderer != null)
        {
            m_ScarletRenderer.enabled = true;
        }
    }

    // dash cannot be cancelled.
    public override void CancelDelay()
    {
        if (m_Hittable != null)
            m_Hittable.RegisterInterject(m_PrevInterject);
    }

    // scarlet is invincible during dash
    public bool OnHit(Damage dmg)
    {
        return true;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashCommand : PlayerCommand {

    public float m_DashDistance = 4.5f;
    public float m_DashTime = 0.05f;
    public float m_DashDelay = 0.15f;

    private Rigidbody m_ScarletBody;

    private void Start()
    {
        m_CommandName = "Dash";
    }

    public override void InitTrigger()
    {
        m_Trigger = new DefaultAxisTrigger(this, m_CommandName);
        m_ScarletBody = m_Scarlet.GetComponent<Rigidbody>();
    }

    public override void TriggerCommand()
    {
        DoDash();
    }
    
    private void DoDash()
    {
        m_Callback.OnCommandStart(m_CommandName, this);
        m_ScarletBody.velocity = new Vector3(0, 0, 0);

        Vector3 dashStart = m_ScarletBody.transform.position;
        Vector3 dashTarget = m_ScarletBody.transform.position + m_ScarletBody.transform.forward * m_DashDistance;

        StartCoroutine(Blink(dashStart, dashTarget));
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
    }

    private void OnDashStart()
    {
        // make invincible
        // @todo: start showing trail / animation
    }

    private void OnDashEnd()
    {
        // stop invincibility
        // @todo: stop showing trail / animation
    }

    // dash cannot be cancelled.
    public override void CancelDelay()
    {
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableAttack : BossAttack {

    public PlayableEffect m_Effect;

    public float m_PlayTime = -1;
    public float m_MoveOnAfter;

    public Vector3 m_EffectLocation;

    protected IEnumerator m_MoveOnTimer;
    protected IEnumerator m_EffectTimer;

    public override void StartAttack()
    {
        base.StartAttack();

        m_Effect.Play(m_EffectLocation);

        if (m_MoveOnAfter <= 0)
            m_Callback.OnAttackEnd(this);
        else
        {
            m_MoveOnTimer = MoveOnAfter(m_MoveOnAfter);
            StartCoroutine(m_MoveOnTimer);
        }

        if (m_PlayTime >= 0)
        {
            m_EffectTimer = StopPlayingAfter(m_PlayTime);
            StartCoroutine(m_EffectTimer);
        }
    }

    protected IEnumerator MoveOnAfter(float time)
    {
        yield return new WaitForSeconds(time);
        m_Callback.OnAttackEnd(this);
    }

    protected IEnumerator StopPlayingAfter(float time)
    {
        yield return new WaitForSeconds(time);

        if (m_Effect != null)
            m_Effect.Hide();
    }

    public override void CancelAttack()
    {
        if (m_MoveOnTimer != null)
            StopCoroutine(m_MoveOnTimer);

        if (m_EffectTimer != null)
            StopCoroutine(m_EffectTimer);

        m_Effect.Hide();
    }


}

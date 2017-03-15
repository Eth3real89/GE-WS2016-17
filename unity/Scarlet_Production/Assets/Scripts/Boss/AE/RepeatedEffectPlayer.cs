using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatedEffectPlayer : BossAttack
{
    public float[] m_Times;

    protected int m_Index;
    protected IEnumerator m_Timer;

    public PlayableEffect m_Effect;
    public float m_TimeBefore = 0.5f;

    public Vector3 m_EffectPosition;

    public override void StartAttack()
    {
        base.StartAttack();

        m_Index = 0;

        m_Timer = WaitThenPlayEffect(m_Times[m_Index]);
        StartCoroutine(m_Timer);
    }

    protected virtual IEnumerator WaitThenPlayEffect(float time)
    {
        yield return new WaitForSeconds(time - m_TimeBefore);
        m_Effect.Play(m_EffectPosition);
        yield return new WaitForSeconds(m_TimeBefore);

        m_Index++;

        if (m_Index < m_Times.Length)
        {
            m_Timer = WaitThenPlayEffect(m_Times[m_Index]);
            StartCoroutine(m_Timer);
        }
    }

    public override void CancelAttack()
    {
        if (m_Timer != null)
            StopCoroutine(m_Timer);

        //m_Effect.Hide();
    }
}

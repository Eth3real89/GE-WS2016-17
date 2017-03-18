using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelDashCloserAttack : AngelAttack {

    public float m_MaxSpeed;
    public Transform m_Scarlet;
    public bool m_LeftOfScarlet;

    public float m_MaxTime = 5f;

    protected IEnumerator m_Enumerator;

    protected FARQ m_Audio;

    public override void StartAttack()
    {
        base.StartAttack();
        m_Enumerator = DoDash();
        StartCoroutine(m_Enumerator);
    }

    protected virtual IEnumerator DoDash()
    {
        m_Audio = FancyAudioEffectsSoundPlayer.Instance.PlayHoverDashSound(transform);

        float t = 0;

        while((t += Time.deltaTime) < AdjustTime(m_MaxTime))
        {
            Vector3 desiredPosition = m_Scarlet.transform.position; //+ new Vector3(1, 0, 0) * 0.5f * (m_LeftOfScarlet ? -1 : 1);
            desiredPosition.y = m_Boss.transform.position.y;

            m_FullTurnCommand.DoTurn();

            if (CloseEnough(desiredPosition))
            {
                break;
            }
            Vector3 movement = CalculateMovement(desiredPosition);
            m_Boss.transform.position += movement.normalized * AdjustSpeed(m_MaxSpeed) * Time.deltaTime;

            yield return null;
        }

        m_Audio.StopIfPlaying();
        EndAttack();
    }

    protected virtual void EndAttack()
    {
        m_Callback.OnAttackEnd(this);
    }

    protected virtual Vector3 CalculateMovement(Vector3 desiredPosition)
    {
        return (desiredPosition - m_Boss.transform.position).normalized;
    }

    protected bool CloseEnough(Vector3 desiredPosition)
    {
        return Vector3.Distance(m_Boss.transform.position, desiredPosition) <= AdjustSpeed(m_MaxSpeed) * Time.deltaTime * 1.5 + 0.5f;
    }

    public override void CancelAttack()
    {
        if (m_Audio != null)
            m_Audio.StopIfPlaying();

        if (m_Enumerator != null)
            StopCoroutine(m_Enumerator);
    }

}

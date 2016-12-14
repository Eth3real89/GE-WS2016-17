using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTowardsScarlet : BossCommand {

    /// <summary>
    /// Max angles (deg) per second.
    /// </summary>
    public float m_TurnSpeed = 1;

    private float _m_AngleTowardsScarlet;

    public float m_AngleTowardsScarlet  {
        get {
            return _m_AngleTowardsScarlet;
        }

        private set
        {
            _m_AngleTowardsScarlet = value;
        }
    }

    public GameObject m_Scarlet;

    public void DoTurn()
    {
        float angle = Mathf.Atan2(
            m_Scarlet.transform.position.x - m_Boss.transform.position.x,
            m_Scarlet.transform.position.z - m_Boss.transform.position.z) * Mathf.Rad2Deg;

        float currentAngle = m_Boss.transform.rotation.eulerAngles.y;

        m_AngleTowardsScarlet = (angle - currentAngle) % 360;
        if (m_AngleTowardsScarlet > 180)
            m_AngleTowardsScarlet -= 360;

        if (m_AngleTowardsScarlet < -180)
            m_AngleTowardsScarlet += 360;

        float turnAngle = CalculateActualTurnAngle(m_AngleTowardsScarlet);

        m_Boss.transform.Rotate(new Vector3(0, 1, 0), turnAngle);
    }

    private float CalculateActualTurnAngle(float totalAngleDifference)
    {
        float turnAngle = totalAngleDifference;

        float maxTurn = m_TurnSpeed * Time.deltaTime;

        if (turnAngle > maxTurn)
        {
            turnAngle = maxTurn;
        }
        else if (turnAngle < -maxTurn)
        {
            turnAngle = -maxTurn;
        }

        return turnAngle;
    }
}

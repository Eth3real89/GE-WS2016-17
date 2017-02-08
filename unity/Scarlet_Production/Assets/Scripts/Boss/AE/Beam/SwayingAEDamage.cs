using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayingAEDamage : BeamAEDamage {

    public int m_NumSways;
    public bool m_InitiallyAimAtScarlet;

    private float m_CurrentAngleDelta;

    protected override IEnumerator RotationRoutine(float time, float angle, ExpandingDamageCallbacks callback)
    {
        float t = 0;
        float prevAngleChange = 0;

        while ((t += Time.deltaTime) < time)
        {
            if (m_InitiallyAimAtScarlet)
            {
                m_CurrentAngleDelta = Mathf.Sin((t / time * m_NumSways / 2f) * 360 * Mathf.Deg2Rad) * angle / 2;
            }
            else
            {
                m_CurrentAngleDelta = Mathf.Cos((t / time * m_NumSways / 2f) * 360 * Mathf.Deg2Rad) * angle / 2 - angle / 2;
            }
                        
            transform.Rotate(Vector3.up, -m_CurrentAngleDelta + prevAngleChange);
            callback.OnRotation(this, transform.eulerAngles.y);

            prevAngleChange = m_CurrentAngleDelta;

            yield return null;
        }

        callback.OnRotationOver(this);
        this.transform.parent = m_OldParent;
    }

    public float GetCurrentAngle()
    {
        return m_CurrentAngleDelta;
    }

}

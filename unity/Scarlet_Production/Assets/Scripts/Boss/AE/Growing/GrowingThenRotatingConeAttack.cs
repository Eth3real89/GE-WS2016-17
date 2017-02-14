using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingThenRotatingConeAttack : GrowingConeAttack
{

    public float m_RotationTime = 15f;
    public float m_RotationAngle = 360;

    protected override void AfterGrow()
    {
        m_GrowEnumerator = Rotate();
        StartCoroutine(m_GrowEnumerator);
    }

    protected virtual IEnumerator Rotate()
    {
        float t = 0;
        while((t += Time.deltaTime) < m_RotationTime)
        {
            m_Boss.transform.Rotate(Vector3.up, m_RotationAngle * Time.deltaTime / m_RotationTime);
            yield return null;
        }

        base.AfterGrow();
    }

}

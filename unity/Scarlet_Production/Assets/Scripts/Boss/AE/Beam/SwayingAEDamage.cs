using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayingAEDamage : BeamAEDamage {

    public int m_NumSways;


    protected override IEnumerator RotationRoutine(float time, float angle, ExpandingDamageCallbacks callback)
    {
        float t = 0;
        float prevAngleChange = 0;

        while ((t += Time.deltaTime) < time)
        {
            float angleDelta = Mathf.Cos((t / time * m_NumSways / 2f) * 360 * Mathf.Deg2Rad) * angle / 2 - angle / 2;
                        
            transform.Rotate(Vector3.up, -angleDelta + prevAngleChange);
            prevAngleChange = angleDelta;

            yield return null;
        }

        callback.OnRotationOver();
    }

}

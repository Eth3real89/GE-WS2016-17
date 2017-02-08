﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAEDamageWonkyMovement : BeamAEDamage {

    public float m_Wonkiness = 20f;
    public int m_NumWobbles = 4;

    protected override IEnumerator RotationRoutine(float time, float angle, ExpandingDamageCallbacks callback)
    {
        float t = 0;
        float prevAngleChange = 0;

        while ((t += Time.deltaTime) < time)
        {
            float angleDelta = t / time * angle + Mathf.Sin(t / time * m_NumWobbles / 2f * 360 * Mathf.Deg2Rad) *m_Wonkiness;
            transform.Rotate(Vector3.up, angleDelta - prevAngleChange);

            prevAngleChange = angleDelta;

            yield return null;
        }

        callback.OnRotationOver(this);
        transform.parent = m_OldParent;
    }


}

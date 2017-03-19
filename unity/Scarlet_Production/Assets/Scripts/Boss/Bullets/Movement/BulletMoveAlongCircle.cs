using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMoveAlongCircle : BulletMovement
{

    public float m_AnglesPerSecond = 90f;

    public Transform m_Center;

    public override void HandleMovement(BulletBehaviour b)
    {
        float distanceToCenter = Vector3.Distance(m_Center.position - new Vector3(0, m_Center.position.y, 0), b.transform.position - new Vector3(0, b.transform.position.y, 0));

        float currentAngle = Mathf.Atan2(b.transform.position.z - m_Center.position.z, b.transform.position.x - m_Center.position.x);
        float newAngle = currentAngle + m_AnglesPerSecond * Time.deltaTime * Mathf.Deg2Rad;

        Vector3 newPos = m_Center.position + distanceToCenter * new Vector3(Mathf.Cos(newAngle), 0, Mathf.Sin(newAngle)).normalized;
        newPos.y = b.transform.position.y;

        b.transform.position = newPos;

        var lookPos = m_Center.position - b.transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        b.transform.rotation = rotation;
    }
}

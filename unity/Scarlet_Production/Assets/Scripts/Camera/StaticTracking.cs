using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticTracking : TrackingBehaviour
{
    public float m_MaxDistance;
    public bool m_FollowPlayer;

    private Transform m_PlayerCameraAnchor;

    private GameObject m_Anchor;

    new void Start()
    {
        base.Start();
        m_PlayerCameraAnchor = m_Player.transform.Find("CameraAnchor");
        m_Anchor = transform.GetChild(0).gameObject;
    }

    public override Vector3 CalculateCameraPosition()
    {
        Vector3 cameraPos = m_Anchor.transform.position;
        if (m_MaxDistance != 0 && Math.Abs(cameraPos.z - m_Player.transform.position.z) > m_MaxDistance)
            cameraPos.z = m_Player.transform.position.z - m_MaxDistance;

        return cameraPos;
    }

    public override Quaternion CalculateCameraRotation()
    {
        if (!m_FollowPlayer)
            return m_Anchor.transform.rotation;
        else
            return Quaternion.LookRotation(m_PlayerCameraAnchor.position - Camera.main.transform.position);
    }
}

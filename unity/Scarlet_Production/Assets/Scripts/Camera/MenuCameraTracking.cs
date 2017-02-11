using System;
using UnityEngine;

public class MenuCameraTracking : TrackingBehaviour
{
    public float m_Distance;
    public float m_OffsetLeft;

    private Transform m_PlayerCameraAnchor;

    new void Start()
    {
        base.Start();
        m_PlayerCameraAnchor = m_Player.transform.Find("CameraAnchor");
    }

    public override Vector3 CalculateCameraPosition()
    {
        Vector3 cameraPos = m_PlayerCameraAnchor.position + (Vector3.back * m_Distance + Vector3.left * m_OffsetLeft);
        return cameraPos;
    }

    public override Quaternion CalculateCameraRotation()
    {
            return Quaternion.LookRotation(Vector3.forward);
    }
}

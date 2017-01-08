using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicTracking : TrackingBehaviour
{
    public float m_HeightOffset;
    public float m_CameraAngle;

    private List<Vector3> m_PathPoints;

    new void Start()
    {
        base.Start();
        m_PathPoints = new List<Vector3>();

        Transform[] pathPointTransforms = GetComponentsInChildren<Transform>();
        foreach (Transform t in pathPointTransforms)
        {
            if (t.GetInstanceID() != transform.GetInstanceID())
            {
                m_PathPoints.Add(t.position);
            }
        }
    }

    public override Vector3 CalculateCameraPosition()
    {
        Vector3 cameraPos = ClosestPointOnLine(m_PathPoints[0], m_PathPoints[1], m_Player.position);
        cameraPos.y = m_Player.transform.position.y + m_HeightOffset;
        return cameraPos;
    }

    public override Quaternion CalculateCameraRotation()
    {
        return Quaternion.Euler(Vector3.forward + new Vector3(m_CameraAngle, 0, 0));
    }

    public Vector3 ClosestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 pnt)
    {
        Vector3 lineDir = lineEnd - lineStart;
        lineDir.Normalize();
        var v = pnt - lineStart;
        var d = Vector3.Dot(v, lineDir);
        return lineStart + lineDir * d;
    }
}

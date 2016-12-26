using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    public GameObject player;
    public GameObject path;
    private List<Vector3> pathPoints;

    void Start()
    {
        pathPoints = new List<Vector3>();
        Transform[] pathPointTransforms = path.GetComponentsInChildren<Transform>();
        foreach (Transform t in pathPointTransforms)
        {
            if (t.GetInstanceID() != path.transform.GetInstanceID())
            {
                pathPoints.Add(t.position);
            }
        }
    }

    void Update()
    {
        Vector3 pos;
        pos = ClosestPointOnLine(pathPoints[0], pathPoints[1], player.transform.position);
        pos.y = 3;
        transform.position = Vector3.Lerp(transform.position, GetBorder(pos, pathPoints[0], pathPoints[1]) - new Vector3(0, 0, 7), .1f);
    }

    private Vector3 GetBorder(Vector3 pos, Vector3 wp1, Vector3 wp2)
    {
        float min = Mathf.Min(wp1.x, wp2.x);
        float max = Mathf.Max(wp1.x, wp2.x);
        if (pos.x > max)
        {
            return new Vector3(max, pos.y, pos.z);
        }
        if (pos.x < min)
        {
            return new Vector3(min, pos.y, pos.z);
        }
        return pos;
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

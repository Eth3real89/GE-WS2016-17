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
        pos.y = player.transform.position.y + 0.4f;
        transform.position = Vector3.Lerp(transform.position, pos, .1f);
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

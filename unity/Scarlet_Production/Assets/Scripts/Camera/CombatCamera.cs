using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CombatCamera : MonoBehaviour
{

    public float m_DampTime = 0.2f;

    public float m_MinDistance = 2.1f;

    public float m_MaxXAngle = 45f;
    public float m_MinXAngle = 25f;

    public float m_YOffset = 0.5f;
    public float m_RotationChangeFactor = 2f;

    /// <summary>
    /// Bigger value = zoom stays closer
    /// </summary>
    public float m_DistanceLogBase = 10f;

    /// <summary>
    /// Bigger value = zoom moves further away
    /// </summary>
    public float m_DistanceMultiplier = 10f;

    private float m_CurrentAngle;

    public GameObject[] m_Targets;

    private Camera m_Camera;
    private float m_ZoomSpeed;
    private Vector3 m_MoveVelocity;

    private float m_ZoomLevel;

    private Vector3 m_AveragePosition;
    private float m_Distance;

    public GameObject m_OtherCamera;

    public GameObject[] m_HideIfInTheWay;
    private List<GameObject> m_HidePlusChildren;

    // Use this for initialization
    void Start()
    {
        m_Camera = GetComponentInParent<Camera>();

        m_HidePlusChildren = new List<GameObject>();
        foreach(GameObject go in m_HideIfInTheWay)
        {
            foreach(Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                m_HidePlusChildren.Add(r.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        FindAveragePosition();
        FindDistance();

        PointCameraToAveragePosition();
        ChangeXRotation();
        UpdateOtherCamera();

        HideObstacles();
    }

    // only when all targets are very close: "zoom in"
    private void ChangeXRotation()
    {
        m_CurrentAngle = m_MinXAngle + (m_Distance / 8f) * Mathf.Abs(m_MaxXAngle - m_MinXAngle);
        m_CurrentAngle = Mathf.Min(m_CurrentAngle, m_MaxXAngle);

        Quaternion newRot = Quaternion.Euler(new Vector3(m_CurrentAngle, 0f, 0f));

        m_Camera.transform.rotation = Quaternion.Slerp(m_Camera.transform.rotation, newRot, Time.deltaTime * m_RotationChangeFactor);
    }

    private void PointCameraToAveragePosition()
    {
        var goalZ = m_Distance * Mathf.Cos(m_CurrentAngle * Mathf.Deg2Rad);
        var goalY = m_Distance * Mathf.Sin(m_CurrentAngle * Mathf.Deg2Rad);

        Vector3 goal = new Vector3(m_AveragePosition.x, m_AveragePosition.y, m_AveragePosition.z);

        goal.z -= goalZ;
        goal.y += goalY + m_YOffset;

        m_Camera.transform.position = Vector3.SmoothDamp(m_Camera.transform.position,
            goal,
            ref m_MoveVelocity, m_DampTime);
    }

    // Calculate the average position of all targets.
    // more or less stolen from Tanks tutorial; difference: we do not calculate the final desired position in only one method; that will be based on what we calculate here, though.
    private void FindAveragePosition()
    {
        Vector3 averagePosition = new Vector3();

        float top = float.NegativeInfinity, left = float.PositiveInfinity, right = float.NegativeInfinity, bottom = float.PositiveInfinity;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (m_Targets[i] == null || !m_Targets[i].activeInHierarchy)
                continue;

            top = Mathf.Max(top, m_Targets[i].transform.position.z);
            left = Mathf.Min(left, m_Targets[i].transform.position.x);
            right = Mathf.Max(right, m_Targets[i].transform.position.x);
            bottom = Mathf.Min(bottom, m_Targets[i].transform.position.z);
        }

        averagePosition = new Vector3(left * 0.5f + right * 0.5f, 0, top * 0.3f + bottom * 0.7f);

        m_AveragePosition = averagePosition;
    }

    private void FindDistance()
    {
        // calculate distance of targets to center to get the distance of the camera.
        // with two possible targets, the distances are always equal.

        float maxDistanceToCenter = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (m_Targets[i] == null || !m_Targets[i].activeInHierarchy)
                continue;

            float distance = Vector3.Distance(m_Targets[i].transform.position, m_AveragePosition);
            maxDistanceToCenter = Mathf.Max(maxDistanceToCenter, distance);
        }

        if (maxDistanceToCenter <= 0.2)
        {
            m_Distance = m_MinDistance;
        }
        else
        {
            m_Distance = m_MinDistance + ((float) Math.Log(maxDistanceToCenter, m_DistanceLogBase) * m_DistanceMultiplier);
        }

        m_Distance = Mathf.Max(m_MinDistance, m_Distance);
    }

    private void UpdateOtherCamera()
    {
        if (m_OtherCamera != null)
        {
            m_OtherCamera.transform.position = m_Camera.transform.position;
            m_OtherCamera.transform.rotation = m_Camera.transform.rotation;
        }
    }

    private void HideObstacles()
    {
        Vector3 copy = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);

        List<GameObject> hitObjects = new List<GameObject>();
        foreach (GameObject obj in m_Targets)
        {
            RaycastHit[] hits = Physics.RaycastAll(copy, Vector3.Normalize(obj.transform.position - copy),
                Vector3.Distance(copy, obj.transform.position) - 0.3f);

            foreach (RaycastHit hit in hits)
            {
                hitObjects.Add(hit.transform.gameObject);
            }

        }

        foreach (GameObject gameObject in m_HidePlusChildren)
        {
            if (hitObjects.IndexOf(gameObject) == -1)
                gameObject.GetComponent<Renderer>().enabled = true;
        }

        foreach (GameObject go in hitObjects)
        {
            if (m_HidePlusChildren.IndexOf(go) != -1)
            {
                go.GetComponent<Renderer>().enabled = false;
            }
        }
    }

}

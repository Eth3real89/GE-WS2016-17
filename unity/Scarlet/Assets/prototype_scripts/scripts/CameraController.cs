using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour {

	public float m_DampTime = 0.2f;
	public float m_ScreenEdgeBuffer = 4f;
	public float m_MinSize = 6.5f;

	public float m_MaxXAngle = 45f;
	public float m_MinXAngle = 15f;

	public GameObject[] m_Targets;

	private Camera m_Camera;
	private float m_ZoomSpeed;
	private Vector3 m_MoveVelocity;

	private float m_ZoomLevel;

	private Vector3 m_AveragePosition;
	private float m_Distance;

	public GameObject m_OtherCamera;

	public GameObject[] m_HideIfInTheWay;

	// Use this for initialization
	void Start () {
		m_Camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
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
		float xAngle = m_MinXAngle + (m_Distance / 8f) * Mathf.Abs(m_MaxXAngle - m_MinXAngle);
		xAngle = Mathf.Min(xAngle, m_MaxXAngle);

		m_Camera.transform.rotation = Quaternion.Slerp(m_Camera.transform.rotation, Quaternion.Euler(new Vector3(xAngle, 0f, 0f)), Time.deltaTime);
	}

	private void PointCameraToAveragePosition()
	{
		float yDistanceRatio = 1f;

		if (m_Distance >= 8f)
			yDistanceRatio = 1f;
		else
		{
			yDistanceRatio = (float) Math.Max(Math.Log10(m_Distance), 0.2f);
			if (yDistanceRatio > 1f) yDistanceRatio = 1f;
		}

		m_Camera.transform.position = Vector3.SmoothDamp(m_Camera.transform.position,
			new Vector3(m_AveragePosition.x, m_AveragePosition.y + m_Distance * yDistanceRatio, m_AveragePosition.z - m_Distance),
			ref m_MoveVelocity, m_DampTime);
	}

	// Calculate the average position of all tanks.
	// more or less stolen from Tanks tutorial; difference: we do not calculate the final desired position in only one method; that will be based on what we calculate here, though.
	private void FindAveragePosition()
	{
		Vector3 averagePosition = new Vector3();
		int numTargets = 0;

		for (int i = 0; i < m_Targets.Length; i++)
		{
			if (!m_Targets[i].activeSelf)
				continue;

			averagePosition += m_Targets[i].transform.position;
			numTargets++;
		}

		if (numTargets > 0)
		   averagePosition /= numTargets;

		m_AveragePosition = averagePosition;
	}

	private void FindDistance()
	{
		// calculate distance of targets to center to get the distance of the camera.
		// with two possible targets, the distances are always equal.

		float maxDistanceToCenter = 0;

		for (int i = 0; i < m_Targets.Length; i++)
		{
			if (!m_Targets[i].activeSelf)
				continue;

			float distance = Vector3.Distance(m_Targets[i].transform.position, m_AveragePosition);
			maxDistanceToCenter = Mathf.Max(maxDistanceToCenter, distance);
		}

		// @todo magic numbers!!
		// @todo change formula!!
		m_Distance = 2f + ((float) Math.Max(Math.Log10((double) maxDistanceToCenter), 0.2) * 10);
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
		GameObject player = GameController.Instance.m_Scarlet;
		GameObject boss = GameController.Instance.m_Boss;

		Vector3 copy = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
				
		RaycastHit[] hits = Physics.RaycastAll(copy, Vector3.Normalize(player.transform.position - copy), 
			Vector3.Distance(copy, player.transform.position) - 0.3f);

		foreach(GameObject gameObject in m_HideIfInTheWay)
		{
			gameObject.GetComponent<Renderer>().enabled = true;
		}

		foreach(RaycastHit hit in hits)
		{
			GameObject go = hit.transform.gameObject;

			if (Array.IndexOf(m_HideIfInTheWay, go) != -1)
			{
				go.GetComponent<Renderer>().enabled = false;
			}
		}
	}

}

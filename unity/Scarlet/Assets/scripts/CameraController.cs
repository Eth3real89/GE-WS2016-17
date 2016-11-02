using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float m_DampTime = 0.2f;
    public float m_ScreenEdgeBuffer = 4f;
    public float m_MinSize = 6.5f;

    public GameObject m_Focus;
    public GameObject m_Scarlet;

    private Camera m_Camera;
    private float m_ZoomSpeed;

    private float m_ZoomLevel;
    private float m_Angle;

	// Use this for initialization
	void Start () {
        m_Camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        // calculate camera position based on player X pos

        float xPos = m_Scarlet.transform.position.x;
        float yPos = m_Camera.transform.position.y;
        float zPos = m_Scarlet.transform.position.z - m_Focus.transform.position.z - 5;

        m_Camera.transform.position = new Vector3(xPos, yPos, zPos);
        m_Camera.transform.LookAt(m_Focus.transform);
	}
}

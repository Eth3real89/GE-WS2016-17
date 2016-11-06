using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

	public float m_HorizontalInput;
	public float m_VerticalInput;

	public float m_Speed = 0.3f;
	public float m_DashDistance;
	public float m_DashSpeed;
	public float m_DashCooldown;

	public Rigidbody m_RigidBody;

	private float m_LastDash;

	public bool m_ControlsEnabled = true;

	private void Awake ()
	{
		m_RigidBody = GetComponent<Rigidbody>();
	}

	// Use this for initialization
	void Start ()
	{
	   
	}
	  

	void setupController()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!m_ControlsEnabled)
			return;

		m_HorizontalInput = Input.GetAxis("Horizontal");
		m_VerticalInput = Input.GetAxis("Vertical");

		Move();
		Rotate();
		CheckDash();
	}

	// move scarlet in the right direction
	void Move()
	{
		float normalizedSpeed = (Mathf.Abs(m_HorizontalInput) + Mathf.Abs(m_VerticalInput)) * m_Speed;
		if (normalizedSpeed >= m_Speed)
		{
			normalizedSpeed = m_Speed;
		}

		Vector3 movement = new Vector3(m_HorizontalInput * normalizedSpeed, 0, m_VerticalInput * normalizedSpeed);
		m_RigidBody.MovePosition(m_RigidBody.position + movement * Time.deltaTime);
	}

	// make sure scarlet is looking in the right direction
	void Rotate()
	{
		// don't want to change where Scarlet is looking when she is not moving
		if (Mathf.Abs(m_HorizontalInput) <= 0.1f && Mathf.Abs(m_VerticalInput) <= 0.1f) return;
		

		float angle = Mathf.Atan2(-m_VerticalInput, m_HorizontalInput);

		Quaternion rotation = Quaternion.Euler(0f, Mathf.Rad2Deg * angle, 0f);
		m_RigidBody.MoveRotation(rotation);
	}

	private void CheckDash()
	{
		if (Input.GetButtonDown("Jump"))
		{
			if (Time.time >= m_LastDash + m_DashCooldown)
			{
				Dash();
				m_LastDash = Time.time;
			}
		}
	}

	private void Dash()
	{
		m_ControlsEnabled = false;
		SetVisibility(false);

		StartCoroutine(Blink());
	}

	private void SetVisibility (bool visible)
	{
		GetComponent<Renderer>().enabled = visible;
		for (int i = 0; i < transform.childCount; i++)
		{
			Renderer r = transform.GetChild(i).GetComponent<Renderer>();
			if (r != null)
				r.enabled = visible;
		}
	}

	private IEnumerator Blink()
	{
		yield return new WaitForSeconds(m_DashSpeed);

		m_RigidBody.MovePosition(m_RigidBody.transform.position + m_RigidBody.transform.right * m_DashDistance);

		m_ControlsEnabled = true;
		SetVisibility(true);
	}

}

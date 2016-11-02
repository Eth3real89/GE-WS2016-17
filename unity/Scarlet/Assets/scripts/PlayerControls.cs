using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {

    public float m_HorizontalInput;
    public float m_VerticalInput;

    public float m_Speed = 0.3f;

    public Rigidbody m_RigidBody;

    private void Awake ()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_HorizontalInput = Input.GetAxis("Horizontal");
        m_VerticalInput = Input.GetAxis("Vertical");

        Move();
        Rotate();
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

}

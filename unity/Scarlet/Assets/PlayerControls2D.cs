using UnityEngine;
using System.Collections;

public class PlayerControls2D : MonoBehaviour
{
    public float m_HorizontalInput;
    public float m_VerticalInput;

    public float m_Speed = 1.3f;

    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        m_HorizontalInput = Input.GetAxis("Horizontal");
        Move();
        Rotate();
    }

    private void Move()
    {
        float normalizedSpeed = m_HorizontalInput * m_Speed;
        if (normalizedSpeed >= m_Speed)
        {
            normalizedSpeed = m_Speed;
        }

        Vector3 movement = new Vector3(m_HorizontalInput * normalizedSpeed, 0, 0);
        rb.velocity = movement;
        animator.SetFloat("Speed", normalizedSpeed);
    }

    // make sure scarlet is looking in the right direction
    void Rotate()
    {
        // don't want to change where Scarlet is looking when she is not moving
        if (Mathf.Abs(m_HorizontalInput) <= 0.1f) return;


        float angle = Mathf.Atan2(m_HorizontalInput, 0);

        Quaternion rotation = Quaternion.Euler(0f, Mathf.Rad2Deg * angle, 0f);
        rb.MoveRotation(rotation);
    }
}

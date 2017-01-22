using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger : MonoBehaviour
{
    public PlayerMoveCommand m_MoveCommand;
    public float m_JumpSpeedThreshold = 4;
    public float m_JumpStrength = 200;

    void Start()
    {
        m_MoveCommand = GameObject.FindGameObjectWithTag("Player").
            GetComponentInChildren<PlayerMoveCommand>();
    }

    private void OnEnable()
    {
        GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Mathf.Abs(other.GetComponent<Rigidbody>().velocity.magnitude) < m_JumpSpeedThreshold)
            {
                return;
            }
            other.GetComponent<Rigidbody>().AddForce(Vector3.up * m_JumpStrength, ForceMode.Impulse);
            StartCoroutine(ReenableJumpTrigger());
            GetComponent<Collider>().enabled = false;
        }
    }

    IEnumerator ReenableJumpTrigger()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<Collider>().enabled = true;
    }
}

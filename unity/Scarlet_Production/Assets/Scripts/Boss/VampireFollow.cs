using UnityEngine;

public class VampireFollow : MonoBehaviour
{
    private GameObject m_Player;
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private bool m_ShouldMove;

    public bool m_Active;

    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!m_Active)
            return;

        Vector3 playerPos = m_Player.transform.position;
        Vector3 selfPos = transform.position;
        playerPos.y = selfPos.y;
        transform.LookAt(playerPos);

        if (Vector3.Distance(playerPos, selfPos) > 2)
            m_ShouldMove = true;
        if (Vector3.Distance(playerPos, selfPos) < 0.7f)
            m_ShouldMove = false;

        if (m_ShouldMove)
        {
            m_Rigidbody.velocity = transform.forward * 1f;
            m_Animator.SetFloat("Speed", 1);
        }
        else
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Animator.SetFloat("Speed", 0);
        }
    }
}

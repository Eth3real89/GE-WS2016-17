using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpField : MonoBehaviour
{
    public float m_WarpDelay;

    private GameObject m_Player;
    private Transform m_Target;
    private bool m_Enabled;

    void Start()
    {
        m_Enabled = true;
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Target = transform.GetChild(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && m_Enabled)
        {
            StartCoroutine(Warp());
        }
    }

    IEnumerator Warp()
    {
        m_Enabled = false;
        m_Player.GetComponent<Rigidbody>().position = m_Target.position;
        yield return new WaitForSeconds(m_WarpDelay);
        m_Enabled = true;
    }
}

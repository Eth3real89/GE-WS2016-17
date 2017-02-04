using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpField : MonoBehaviour
{
    public float m_WarpDelay;

    private GameObject m_Player;
    private Transform m_Target;
    private DontGoThroughThings m_Blocker;

    void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Blocker = m_Player.GetComponent<DontGoThroughThings>();
        m_Target = transform.GetChild(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(Warp());
        }
    }

    IEnumerator Warp()
    {
        m_Player.SetActive(false);
        m_Player.transform.position = m_Target.position;
        m_Blocker.ResetPosition();
        yield return new WaitForSeconds(m_WarpDelay);
        m_Player.SetActive(true);
    }
}

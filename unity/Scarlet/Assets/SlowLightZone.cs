using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowLightZone : MonoBehaviour {

    public float m_SlowBy = 0.3f;
    public bool m_DisableDash = true;

    private bool m_SlowScarlet = false;
    private Collider m_ScarletCollider;
    private PlayerControlsCharController m_ScarletController;

    void Start()
    {
        m_ScarletCollider = GameController.Instance.m_Scarlet.GetComponent<Collider>();
        m_ScarletController = GameController.Instance.m_Scarlet.GetComponentInChildren<PlayerControlsCharController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != m_ScarletCollider || m_SlowScarlet)
            return;

        SlowScarlet();
    }
	
	private void OnTriggerStay(Collider other)
    {
        if (other != m_ScarletCollider || m_SlowScarlet)
            return;

        SlowScarlet();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != m_ScarletCollider || !m_SlowScarlet)
            return;

        RestoreScarlet();
    }

    private void SlowScarlet()
    {
        m_SlowScarlet = true;
        m_ScarletController.m_SpeedRun *= (1 - m_SlowBy);
        m_ScarletController.m_DashEnabled = false;
    }

    private void RestoreScarlet()
    {
        m_SlowScarlet = false;
        m_ScarletController.m_SpeedRun /= (1 - m_SlowBy);
        m_ScarletController.m_DashEnabled = true;
    }
}

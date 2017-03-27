using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTextCreditsFadeInController : MonoBehaviour {
    private bool m_FadedIn = false;
    private AreaEnterTextController m_Controller;
	// Use this for initialization
	void Start () {
        m_Controller = GetComponentInChildren<AreaEnterTextController>();
        
    }
    void Update()
    {
        if (m_Controller != null && !m_FadedIn)
        {
            m_FadedIn = true;
            m_Controller.StartFadeIn();
        }
    }
}

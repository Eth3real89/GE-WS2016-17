using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private static CameraController _Instance;
    
    public static CameraController Instance
    {
        get
        {
            return _Instance;
        }
    }

    public CombatCamera m_DefaultCamera;
    public CombatCamera m_ZoomCamera;
    public CombatCamera m_TopDownCamera;

	void Start () {
        if (_Instance == null)
        {
            _Instance = this;
        }		
	}

    public void ZoomIn()
    {
        SetCamerasEnabled(true, m_ZoomCamera);
        SetCamerasEnabled(false, m_DefaultCamera, m_TopDownCamera);
    }

    public void ZoomOut()
    {
        SetCamerasEnabled(true, m_TopDownCamera);
        SetCamerasEnabled(false, m_DefaultCamera, m_ZoomCamera);
    }

    public void ActivateDefaultCamera()
    {
        SetCamerasEnabled(true, m_DefaultCamera);
        SetCamerasEnabled(false, m_ZoomCamera, m_TopDownCamera);
    }

    private void SetCamerasEnabled(bool enabled, params CombatCamera[] cameras)
    {
        foreach(CombatCamera camera in cameras)
        {
            camera.gameObject.SetActive(enabled);
        }
    }

    public void Darken(bool darken)
    {
        DarkenCameraImage dci = GetComponent<DarkenCameraImage>();
        if (dci != null)
            dci.enabled = darken;
    }
    
}

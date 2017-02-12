using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadBossFight : MonoBehaviour
{
    public string m_SceneName;

    private void OnTriggerEnter(Collider other)
    {
        LevelManager.Instance.m_ControlMode = LevelManager.ControlMode.Combat;
        SceneManager.LoadScene(m_SceneName);

        LevelManager.Instance.QuickLoadFix();
    }
}

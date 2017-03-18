using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadBossFight : MonoBehaviour
{
    public string m_SceneName;
    public bool m_FadeToBlack;
    private FadeToBlack.FadeFinishedCallback m_FadeCallback;

    private void OnTriggerEnter(Collider other)
    {
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        m_FadeCallback = LoadScene;

        if (m_FadeToBlack)
            Camera.main.GetComponent<FadeToBlack>().StartFade(Color.black, 2, m_FadeCallback);
        else
            LoadScene();
    }

    private void LoadScene()
    {
        LevelManager.Instance.m_ControlMode = LevelManager.ControlMode.Combat;
        SceneManager.LoadScene(m_SceneName);

        LevelManager.Instance.QuickLoadFix();
    }
}

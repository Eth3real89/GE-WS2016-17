using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour {

    public AudioSource sound;

    private bool m_Started = false;

	// Use this for initialization
	void Start () {
        m_Started = false;
	}

    void Update()
    {
        if (!m_Started)
        {
            if (Input.anyKey)
            {
                sound.Play();
                StartCoroutine(WhileSceneIsLoading());
                m_Started = true;
            }
        }
    }

    private IEnumerator WhileSceneIsLoading()
    {
        AsyncOperation m_SceneLoadingOperation = SceneManager.LoadSceneAsync(0);
        m_SceneLoadingOperation.allowSceneActivation = false;

        while (m_SceneLoadingOperation.progress < 0.89)
        {
            yield return null;
        }
        m_SceneLoadingOperation.allowSceneActivation = true;
    }

    public void OnStartGame()
    {
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour {

    public AudioSource sound;

	// Use this for initialization
	void Start () {
	
	}

    void Update()
    {
        if (Input.anyKey)
        {
            sound.Play();
            SceneManager.LoadScene(0);
        }
    }

    public void OnStartGame()
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameMenuController : MonoBehaviour {



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnResume(GameObject resumeButton)
    {
        resumeButton.SetActive(false);
        //Menü unsichtbar machen
    }

    public void OnChangeVolume(int volume)
    {
        //Musik Lautstärke auf den übergebenen Wert setzen
    }

    public void OnChangeSoundVolume(int volume)
    {
        //Sounds Lautstärke auf den übergebebene Wert setzen
    }

    public void OnInvertControl(bool inverted)
    {

        //Je nach übergebenen Wert die Steuerung invertieren oder nicht
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("userinterface_menu");

        //Zurück zum Hauptmenü
    }



}

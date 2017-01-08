using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameMenuController : MonoBehaviour {

    public GameObject menu;
    private bool menuVisible = false;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Cancel"))
        {
            if (menuVisible)
            {
                menuVisible = false;
                menu.SetActive(false);
            }
            else
            {
                menuVisible = true;
                menu.SetActive(true);
            }
        }
	}

    public void OnResume()
    {
        menuVisible = false;
        menu.SetActive(false);
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

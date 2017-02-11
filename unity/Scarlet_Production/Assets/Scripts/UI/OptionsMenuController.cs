using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour {

    public GameObject[] MenuItems;

    private int music = 0;
    private int sound = 1;
    private int backToMain = 2;

    public GameObject menu;
    private int selected;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Vertical"))
        {
            if (Input.GetAxis("Vertical") < 0)
            {
                if (selected == MenuItems.Length - 1)
                {
                    selected = music;
                }
                else
                {
                    selected += 1;
                }
            }
            else
            {
                if (selected == music)
                {
                    selected = MenuItems.Length - 1;
                }
                else
                {
                    selected -= 1;
                }
            }
            SelectItem(selected);
        }
        if (Input.GetButtonDown("Horizontal"))
        {
            if (selected == music)
            {
                if (Input.GetAxis("Horizontal") < 0)
                {
                    MenuItems[selected].GetComponentInChildren<Slider>().value = MenuItems[selected].GetComponentInChildren<Slider>().value - 1;
                }
                else
                {
                    MenuItems[selected].GetComponentInChildren<Slider>().value = MenuItems[selected].GetComponentInChildren<Slider>().value + 1;
                }
            }
            else if (selected == sound)
            {
                if (Input.GetAxis("Horizontal") < 0)
                {
                    MenuItems[selected].GetComponentInChildren<Slider>().value = MenuItems[selected].GetComponentInChildren<Slider>().value - 1;
                }
                else
                {
                    MenuItems[selected].GetComponentInChildren<Slider>().value = MenuItems[selected].GetComponentInChildren<Slider>().value + 1;
                }

            }

        }
        if (Input.GetButtonDown("Submit"))
        {
            if (selected == backToMain)
            {
                BackToMain();
            }
        }
    }

    public void BackToMain()
    {
        if (selected == backToMain)
        {
            //Zurück zum Hauptmenü
            menu.SetActive(false);
            GetComponent<MainMenuController>().enabled = true;
            GetComponent<MainMenuController>().Activate();
            GetComponent<OptionsMenuController>().enabled = false;
        }
    }

    public void Activate()
    {
        menu.SetActive(true);
    }

    public void SelectItem(int itemNumber)
    {
        if (selected != itemNumber)
        {
            selected = itemNumber;
        }
        for (int i = 0; i < MenuItems.Length; i++)
        {
            Image background = MenuItems[i].GetComponentInChildren<Image>();
            if (itemNumber == i)
            {
                background.sprite = Resources.Load<Sprite>("BackgroundPanelSelected");
            }
            else
            {
                background.sprite = Resources.Load<Sprite>("BackgroundPanel");
            }
        }

    }
}

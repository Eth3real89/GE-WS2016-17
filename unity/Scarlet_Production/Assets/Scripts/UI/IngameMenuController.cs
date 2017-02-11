using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameMenuController : MonoBehaviour
{

    public GameObject[] MenuItems;

    public GameObject menu;
    private bool menuVisible = false;
    private int selected;

    // Use this for initialization
    void Start()
    {
        selected = 0;
        SelectItem(selected);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuVisible)
            {
                menuVisible = false;
                menu.SetActive(false);
                SetScarletControlsEnabled(true);
            }
            else
            {
                menuVisible = true;
                menu.SetActive(true);
                SetScarletControlsEnabled(false);
            }
        }
        if(Input.GetButtonDown("Vertical"))
        {
            if (Input.GetAxis("Vertical") < 0)
            {
                if(selected == MenuItems.Length - 1)
                {
                    selected = 0;
                } else
                {
                    selected += 1;
                }
            }
            else
            {
                if (selected == 0)
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
        if(Input.GetButtonDown("Horizontal"))
        {
            if (selected == 1)
            {
                if(Input.GetAxis("Horizontal") < 0)
                {
                    MenuItems[selected].GetComponentInChildren<Slider>().value = MenuItems[selected].GetComponentInChildren<Slider>().value - 1;
                } else
                {
                    MenuItems[selected].GetComponentInChildren<Slider>().value = MenuItems[selected].GetComponentInChildren<Slider>().value + 1;
                }
            }
            else if (selected == 2)
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
            if (selected == 0)
            {
                OnResume();
            }
            else if (selected == 4)
            {
                BackToMain();
            }
        }
    }

    public void OnResume()
    {
        if(selected == 0)
        {
            menuVisible = false;
            menu.SetActive(false);
            SetScarletControlsEnabled(true);
        }
    }

    private void OnChangeMusicVolume(int volume)
    {
        if(selected == 1)
        {
            //Musik Lautstärke auf den übergebenen Wert setzen
        }
    }

    private void OnChangeSoundVolume(int volume)
    {
        if (selected == 2)
        {
            //Sounds Lautstärke auf den übergebebenen Wert setzen
        }
    }


    public void ToggleControllerInput() {
        if (selected == 3)
        {
            //Je nach Wert die Steuerung invertieren oder nicht
        }
    }

    private void BackToMain()
    {
        if (selected == 4)
        {
            //Zurück zum Hauptmenü
            SceneManager.LoadScene("userinterface_menu");
        }
    }

    private void SetScarletControlsEnabled(bool enabled)
    {
        PlayerControls controls = FindObjectOfType<PlayerControls>();
        if (controls != null)
        {
            if (enabled)
                controls.EnableAllCommands();
            else
                controls.DisableAllCommands();
        }
    }

    public void SelectItem(int itemNumber)
    {
        if (selected != itemNumber)
        {
            selected = itemNumber;
        }
        for (int i=0; i<MenuItems.Length; i++)
        {
            Image background = MenuItems[i].GetComponentInChildren<Image>();
            if (itemNumber == i)
            {
                background.sprite = Resources.Load<Sprite>("BackgroundPanelSelected");
            } else
            {
                background.sprite = Resources.Load<Sprite>("BackgroundPanel");
            }
        }

    }
}

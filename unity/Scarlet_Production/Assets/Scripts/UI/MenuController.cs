using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    //Main 0=Start; 1=Load; 2=Options; 3=Leave
    public GameObject[] MenuItems;
    
    public GameObject menu;
    private int selected;


    // Use this for initialization
    void Start ()
    {
        selected = 0;
        SelectItem(selected);
        SetScarletControlsEnabled(false);
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("Vertical"))
        {
            if (Input.GetAxis("Vertical") < 0)
            {
                if (selected == MenuItems.Length - 1)
                {
                    selected = 0;
                }
                else
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

        if (Input.GetButtonDown("Submit"))
        {
            if (selected == 0)
            {
                StartNewGame();
            }
            else if (selected == 1)
            {
                LoadGame();
            }
            else if (selected == 2)
            {
                OpenOptions();
            }
            else if (selected == 3)
            {
                CloseGame();
            }
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

    private void ZoomToScarlet()
    {
        SetScarletControlsEnabled(true);
    }

    public void StartNewGame()
    {
        if(selected == 0)
        {
            ZoomToScarlet();
            menu.SetActive(false);
        }
    }

    public void LoadGame()
    {
        if (selected == 1)
        {
            ZoomToScarlet();
            menu.SetActive(false);
        }
    }

    public void OpenOptions()
    {
        if (selected == 2)
        {
            menu.SetActive(false);
            GetComponent<OptionsMenuController>().enabled = true;
            GetComponent<OptionsMenuController>().Activate();
            GetComponent<MenuController>().enabled = false;
        }
    }

    public void Activate()
    {
        menu.SetActive(true);
    }

    public void CloseGame()
    {
        if (selected == 3)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
    
}

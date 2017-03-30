using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectMenuController : MonoBehaviour {

    public GameObject[] MenuItems;
    public GameObject menu;

    private const int m_ScarletSuburb = 0;
    private const int m_CrimsonCopse = 1;
    private const int m_SanguineShelter = 2;
    private const int m_MaroonMonastery = 3;
    private const int backToMain = 4;

    private int m_CurrentLevel;
    private string m_SceneName;
    private int selected;

    private float m_Pressed;

    // Use this for initialization
    void Start()
    {
        selected = m_ScarletSuburb;
        SelectItem(selected);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Parry"))
        {
            BackToMain();
        }
        float pressed = Input.GetAxis("Vertical");

        if ((pressed != 0 && !((pressed > 0 && m_Pressed > 0) || (pressed < 0 && m_Pressed < 0))) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (pressed < 0 || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selected == MenuItems.Length - 1)
                {
                    selected = m_ScarletSuburb;
                }
                else
                {
                    selected += 1;
                }
            }
            else
            {
                if (selected == m_ScarletSuburb)
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
        m_Pressed = pressed;
        if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Attack"))
        {
            AskIfLoadNewLevel(selected);
        }
    }

    public void EnableMenu()
    {
        enabled = true;
    }

    public void AskIfLoadNewLevel(int level)
    {
        m_CurrentLevel = level;
        if(level == backToMain || PlayerPrefs.GetString("CurrentLevel").Equals(""))
        {
            Load();
        } else
        {
            enabled = false;
            FindObjectOfType<WarningBoxController>().StartFadeIn("Are you sure? Your progress will be lost!", "Yes", "No", Load, EnableMenu);
        }
    }


    private void LoadScene()
    {
        SceneManager.LoadScene(m_SceneName);

        LevelManager.Instance.QuickLoadFix();
    }

    public void Load()
    {
        switch (m_CurrentLevel)
        {
            case m_ScarletSuburb:
                m_SceneName = "city_exploration_level";
                Camera.main.GetComponent<FadeToBlack>().StartFade(Color.black, 2, LoadScene);
                break;
            case m_CrimsonCopse:
                m_SceneName = "forest_exploration_level";
                Camera.main.GetComponent<FadeToBlack>().StartFade(Color.black, 2, LoadScene);
                break;
            case m_SanguineShelter:
                m_SceneName = "post_forest_exploration_level";
                Camera.main.GetComponent<FadeToBlack>().StartFade(Color.black, 2, LoadScene);
                break;
            case m_MaroonMonastery:
                m_SceneName = "maze_exploration_level";
                Camera.main.GetComponent<FadeToBlack>().StartFade(Color.black, 2, LoadScene);
                break;
            case backToMain:
                BackToMain();
                break;
            default:
                BackToMain();
                break;
        }

    }
        

    public void BackToMain()
    {
        //Zurück zum Hauptmenü
        menu.SetActive(false);
        GetComponent<MainMenuController>().enabled = true;
        GetComponent<MainMenuController>().Activate(null);
        GetComponent<LevelSelectMenuController>().enabled = false;
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

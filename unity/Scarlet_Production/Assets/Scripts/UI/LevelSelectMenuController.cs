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

    
    private int selected;

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
        if (Input.GetButtonDown("Vertical") || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Input.GetAxis("Vertical") < 0 || Input.GetKeyDown(KeyCode.DownArrow))
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
        if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Attack"))
        {
            Load(selected);
        }
    }

    public void Load(int level)
    {
        switch (level)
        {
            case m_ScarletSuburb:
                SceneManager.LoadScene("city_exploration_level");
                break;
            case m_CrimsonCopse:
                SceneManager.LoadScene("forest_exploration_level");
                break;
            case m_SanguineShelter:
                SceneManager.LoadScene("post_forest_exploration_level");
                break;
            case m_MaroonMonastery:
                SceneManager.LoadScene("maze_exploration_level");
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

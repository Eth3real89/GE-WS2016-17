using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour {

    public GameObject[] MenuItems;
    public GameObject menu;

    private int music = 0;
    private int backToMain = 1;


    private Slider m_MusicSlider;
    private int selected;

    // Use this for initialization
    void Start () {
        selected = 0;
        m_MusicSlider = MenuItems[music].GetComponentInChildren<Slider>();
        SelectItem(selected);
        if (m_MusicSlider.value != PlayerPrefs.GetFloat("CurrentVolume") * m_MusicSlider.maxValue)
        {
            m_MusicSlider.value = PlayerPrefs.GetFloat("CurrentVolume") * m_MusicSlider.maxValue;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("Vertical") || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Input.GetAxis("Vertical") < 0 || Input.GetKeyDown(KeyCode.DownArrow))
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
        if (Input.GetButton("Horizontal") || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            if (selected == music)
            {
                if (Input.GetAxis("Horizontal") < 0 || Input.GetKey(KeyCode.LeftArrow))
                {
                    m_MusicSlider.value = m_MusicSlider.value - 1;
                    //AudioListener.volume = m_MusicSlider.value / m_MusicSlider.maxValue;
                    //PlayerPrefs.SetFloat("CurrentVolume", AudioListener.volume);
                }
                else
                {
                    m_MusicSlider.value = m_MusicSlider.value + 1;
                    //AudioListener.volume = m_MusicSlider.value / m_MusicSlider.maxValue;
                    //PlayerPrefs.SetFloat("CurrentVolume", AudioListener.volume);
                }
            }

        }
        if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Attack"))
        {
            if (selected == backToMain)
            {
                BackToMain();
            }
        }
    }

    public void SetVolume()
    {
        AudioListener.volume = m_MusicSlider.value / m_MusicSlider.maxValue;
        PlayerPrefs.SetFloat("CurrentVolume", AudioListener.volume);
    }

    public void BackToMain()
    {
        if (selected == backToMain)
        {
            //Zurück zum Hauptmenü
            menu.SetActive(false);
            GetComponent<MainMenuController>().enabled = true;
            GetComponent<MainMenuController>().Activate(null);
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

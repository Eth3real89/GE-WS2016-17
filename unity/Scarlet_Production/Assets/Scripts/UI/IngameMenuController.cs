using UnityEngine;
using UnityEngine.UI;

public class IngameMenuController : MonoBehaviour
{
    public TrackingBehaviour menuCamera;
    public GameObject[] MenuItems;
    public GameObject menu;
    public bool m_InCombatScene = false;

    private int resume = 0;
    private int music = 1;
    private int backToMain = 2;


    private Slider m_MusicSlider;

    private bool menuVisible = false;
    private int selected;
    private CameraTracking cameraTracking;
    private TrackingBehaviour previousTracking;
    private TrackingBehaviour normalTracking;

    void Start()
    {
        if(!m_InCombatScene)
        {
            cameraTracking = Camera.main.GetComponent<CameraTracking>();
        }
        selected = resume;
        m_MusicSlider = MenuItems[music].GetComponentInChildren<Slider>();
        SelectItem(selected);
        if (m_MusicSlider.value != PlayerPrefs.GetFloat("CurrentVolume") * m_MusicSlider.maxValue)
        {
            m_MusicSlider.value = PlayerPrefs.GetFloat("CurrentVolume") * m_MusicSlider.maxValue;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuVisible)
            {
                ContinueGame();
            }
            else
            {
                PauseGame();
            }
        }
        if(menuVisible && Input.GetButtonDown("Parry"))
        {
            ContinueGame();
        }

        if (menuVisible)
        {
            if (Input.GetButtonDown("Vertical") || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (Input.GetAxis("Vertical") < 0 || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (selected == MenuItems.Length - 1)
                    {
                        selected = resume;
                    }
                    else
                    {
                        selected += 1;
                    }
                }
                else
                {
                    if (selected == resume)
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
                if (selected == resume)
                {
                    OnResume();
                }
                else if (selected == backToMain)
                {
                    BackToMain();
                }
            }
        }
    }

    private void ContinueGame()
    {
        if (!m_InCombatScene)
        {
            cameraTracking.m_TrackingBehaviour = previousTracking;
        }
        menuVisible = false;
        menu.SetActive(false);
        SetScarletControlsEnabled(true);
    }

    private void PauseGame()
    {

        if (!m_InCombatScene)
        {
            previousTracking = cameraTracking.m_TrackingBehaviour;
            normalTracking = previousTracking;
            cameraTracking.m_TrackingBehaviour = menuCamera;
        }
        menuVisible = true;
        menu.SetActive(true);
        SetScarletControlsEnabled(false);
    }

    public void OnResume()
    {
        if (selected == resume)
        {
            ContinueGame();
        }
    }

    public void OnChangeMusicVolume()
    {
        //Musik Lautstärke auf den übergebenen Wert setzen
        AudioListener.volume = m_MusicSlider.value / m_MusicSlider.maxValue;
        PlayerPrefs.SetFloat("CurrentVolume", m_MusicSlider.value / m_MusicSlider.maxValue);
    }

    public void BackToMain()
    {
        if (selected == backToMain)
        {
            //Zurück zum Hauptmenü
            menu.SetActive(false);
            menuVisible = false;
            GetComponent<IngameMenuController>().enabled = false;
            GetComponentInChildren<MainMenuController>(true).enabled = true;

            if (!m_InCombatScene)
            {
                GetComponentInChildren<MainMenuController>(true).Activate(normalTracking);
            } else
            {
                GetComponentInChildren<MainMenuController>(true).Activate(null);
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

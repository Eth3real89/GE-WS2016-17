using SequencedActionCreator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    //Main 0=Start; 1=Load; 2=Options; 3=Leave
    public GameObject[] MenuItems;
    public TrackingBehaviour menuCamera;
    public GameObject Menu;

    public bool isShowing = false;

    private int selected;
    private CameraTracking cameraTracking;
    private TrackingBehaviour previousTracking;
    private TrackingBehaviour normalTracking;
    private bool m_FirstStartWhileSceneOpen = true;

    void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("CurrentVolume", 1);
        cameraTracking = Camera.main.GetComponent<CameraTracking>();
        
        if (isShowing && PlayerPrefs.GetInt("IsStarted") == 0)
            {
            previousTracking = cameraTracking.m_TrackingBehaviour;
            normalTracking = previousTracking;
            cameraTracking.m_TrackingBehaviour = menuCamera;
            
            selected = 0;
            SelectItem(selected);
            Menu.SetActive(true);
            SetScarletControlsEnabled(false);
        } 
         else if(PlayerPrefs.GetInt("IsStarted") == 1)
        {
            StartNewGame();
        } else if(!isShowing && FindObjectOfType<AreaEnterTextController>() != null)
        {
            FindObjectOfType<AreaEnterTextController>().StartFadeIn();
        }
    }

    void Update()
    {

        if (isShowing)
        {
            if (Input.GetButtonDown("Vertical") || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (Input.GetAxis("Vertical") < 0 || Input.GetKeyDown(KeyCode.DownArrow))
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
        isShowing = false;
        if(normalTracking != null)
        {
            cameraTracking.m_TrackingBehaviour = normalTracking;
        } else
        {
            cameraTracking.m_TrackingBehaviour = previousTracking;
        }
        SetScarletControlsEnabled(true);
    }

    public void StartNewGame()
    {
        if (selected == 0)
        {
            // Reset Player Prefs except volume
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetFloat("CurrentVolume", AudioListener.volume);
            if (SceneManager.GetActiveScene().name.Equals("city_exploration_level") && m_FirstStartWhileSceneOpen)
            {
                m_FirstStartWhileSceneOpen = false;
                ZoomToScarlet();
                Menu.SetActive(false);
                GetComponent<MainMenuController>().enabled = false;
                GetComponentInParent<IngameMenuController>().enabled = true;

                ActivateScarletControls();
                SequencedActionController.Instance.PlayCutscene("Opening");
            } else
            {
                PlayerPrefs.SetInt("IsStarted", 1);
                SceneManager.LoadScene("city_exploration_level");
            }
        }
    }

    public void LoadGame()
    {
        if (selected == 1)
        {
            ////TODO: Check load game with checkpoints
            var currentScene = SceneManager.GetActiveScene().name;
            if (!PlayerPrefs.GetString("CurrentLevel", "city_exploration_level").Equals(currentScene))
            {
                SceneManager.LoadScene(PlayerPrefs.GetString("CurrentLevel", "city_exploration_level"));
            }
            else
            {
                ZoomToScarlet();
                Menu.SetActive(false);
                GetComponent<MainMenuController>().enabled = false;
                GetComponentInParent<IngameMenuController>().enabled = true;

                ActivateScarletControls();
                //if (currentScene.Equals("city_exploration_level"))
                //{
                //    SequencedActionController.Instance.PlayCutscene("Opening");
                //}
            }
        }
    }

    public void OpenOptions()
    {
        if (selected == 2)
        {
            Menu.SetActive(false);
            GetComponent<OptionsMenuController>().enabled = true;
            GetComponent<OptionsMenuController>().Activate();
            GetComponent<MainMenuController>().enabled = false;
        }
    }

    public void Activate()
    {
        isShowing = true;

        previousTracking = cameraTracking.m_TrackingBehaviour;
        cameraTracking.m_TrackingBehaviour = menuCamera;
        selected = 0;
        SelectItem(selected);
        Menu.SetActive(true);
        SetScarletControlsEnabled(false);
    }

    private void ActivateScarletControls()
    {
        ControlsActivator activator = FindObjectOfType<ControlsActivator>();
        if (activator != null)
            activator.ActivateControls();
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

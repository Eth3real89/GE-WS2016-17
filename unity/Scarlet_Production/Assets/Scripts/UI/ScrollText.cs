using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollText : MonoBehaviour
{
    public Camera camera;
    public RectTransform image1, image2;
    public float offset, offsetImages, speed;
    public bool cutsceneStarted;
    public LoadBossFight credits;
    public bool m_showCloseInteraction = false;

    private float screenHeight;
    private RectTransform rect;
    private Text text;
    private bool startedSceneLoad;

    private void Start()
    {
        text = GetComponent<Text>();
        rect = GetComponent<RectTransform>();
        rect.localPosition = new Vector2(0, camera.pixelHeight / -2 + offset);
        image1.localPosition = new Vector2(0, image1.localPosition.y + camera.pixelHeight / -2 + offset);
        image2.localPosition = new Vector2(0, image2.localPosition.y + camera.pixelHeight / -2 + offset);
        if(m_showCloseInteraction)
        {
            FindObjectOfType<ShowRewardMessageController>().StartFadeIn("", true);
        }
    }

    public void CutsceneStarted()
    {
        cutsceneStarted = true;
    }

    private void Update()
    {
        Vector3 velocity = new Vector3(0, speed * Time.deltaTime, 0);
        rect.localPosition += velocity;

        image1.localPosition += velocity;

        if (image2.localPosition.y < 0)
            image2.localPosition += velocity;
        else if (!cutsceneStarted)
        {
            image2.localPosition += velocity;
        }

        if (!cutsceneStarted && image2.localPosition.y > camera.pixelHeight / 2 && !startedSceneLoad)
        {
            startedSceneLoad = true;
            credits.LoadNextSceneSlowFade();
            FindObjectOfType<FadeOutAudio>().FadeOut();
        }
    }
}

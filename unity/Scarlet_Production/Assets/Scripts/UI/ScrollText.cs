using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollText : MonoBehaviour
{
    public Camera camera;
    public RectTransform images;
    public float offset, offsetImages, speed;
    public bool cutsceneStarted;
    public LoadBossFight credits;

    private float screenHeight;
    private RectTransform rect;
    private Text text;
    private bool startedSceneLoad;

    private void Start()
    {
        text = GetComponent<Text>();
        rect = GetComponent<RectTransform>();
        rect.localPosition = new Vector2(0, camera.pixelHeight / -2 + offset);
        images.localPosition = new Vector2(0, rect.localPosition.y + offsetImages - text.preferredHeight);
    }

    void Update()
    {
        Vector3 velocity = new Vector3(0, speed * Time.deltaTime, 0);
        rect.localPosition += velocity;

        if (images.localPosition.y < 0)
            images.localPosition += velocity;
        else if (!cutsceneStarted)
        {
            images.localPosition += velocity;
        }

        if (!cutsceneStarted && images.localPosition.y > camera.pixelHeight / 2 && !startedSceneLoad)
        {
            startedSceneLoad = true;
            credits.LoadNextSceneSlowFade();
        }
    }
}

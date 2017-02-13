using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPromptController : MonoBehaviour {

    private bool inArea;
    private GameObject WorldObject;
    private Camera cam;
    private RectTransform ui_element;
    private RectTransform CanvasRect;

    // Use this for initialization
    void Start () {

        cam = FindObjectOfType<Camera>();
        ui_element = gameObject.GetComponent<RectTransform>();
        CanvasRect = GameObject.Find("UI").GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {
        if(inArea)
        {
            Map3DTo2D();
        }
	}

    public void IsInTriggerArea(GameObject WorldObject, bool inArea)
    {
        this.inArea = inArea;
        this.WorldObject = WorldObject;
    }

    private void Map3DTo2D()
    {
        if(WorldObject != null)
        {
            Vector2 ViewportPosition = cam.WorldToViewportPoint(WorldObject.transform.position);
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

            //now you can set the position of the ui element
            ui_element.anchoredPosition = WorldObject_ScreenPosition;
        }
    }

}

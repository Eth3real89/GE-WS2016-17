using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHighlightController : MonoBehaviour, IPointerEnterHandler
{
    public int index;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInParent<IngameMenuController>().SelectItem(index);
    }

}

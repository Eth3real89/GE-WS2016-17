
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHighlightController : MonoBehaviour, IPointerEnterHandler
{
    public int index;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInParent<MenuController>().SelectItem(index);
    }
}


using UnityEngine;
using UnityEngine.EventSystems;

public class IngameMenuButtonHighlightController : MonoBehaviour, IPointerEnterHandler
{
    public int index;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInParent<IngameMenuController>().SelectItem(index);
    }

}

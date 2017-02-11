
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenuButtonHighlightController : MonoBehaviour, IPointerEnterHandler
{

    public int index;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInParent<OptionsMenuController>().SelectItem(index);
    }

}

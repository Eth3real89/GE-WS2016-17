using UnityEngine;
using UnityEngine.EventSystems;

public class LevelSelectMenuButtonHighlightController : MonoBehaviour, IPointerEnterHandler
{
    public int index;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInParent<LevelSelectMenuController>().SelectItem(index);
    }
    
}

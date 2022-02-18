using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class JourneyChestBehaviour : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool wasChestSelected;
    public JourneyChest chest;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (wasChestSelected)
            return;

        transform.DOScale(Vector3.one * 1.2f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (wasChestSelected)
            return;

        transform.DOScale(Vector3.one, 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (wasChestSelected)
            return;

        chest.Select();
        JourneyChestManager.instance.ChestWasSelected(chest);
    }

}

using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
public class JourneyNodeBehaviour : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public JourneyNode journeyNode;
    JourneyMapManager journeyMapManager;

    Vector3 originalScale;

    public void Initialize(JourneyNode _jn)
    {
        journeyNode = _jn;
        journeyMapManager = JourneyMapManager.instance;
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (journeyMapManager.availableNodes.Contains(journeyNode))
            transform.DOPause();
        else
            transform.DOScale(originalScale * 1.2f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (journeyMapManager.availableNodes.Contains(journeyNode))
            transform.DOPlay();
        else
            transform.DOScale(originalScale, 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        journeyMapManager.NodeClick(this);
    }

}

using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Threading.Tasks;

public class JourneyNodeBehaviour : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public JourneyNode journeyNode;
    JourneyMapManager journeyMapManager;
    JourneyMapUI journeyMapUI;
    SpriteRenderer sr;

    Vector3 originalScale;

    public void Initialize(JourneyNode _jn)
    {
        journeyNode = _jn;
        journeyMapManager = JourneyMapManager.instance;
        journeyMapUI = journeyMapManager.GetComponent<JourneyMapUI>();

        sr = GetComponentInChildren<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    public void AnimateAvailableNode()
    {
        float duration = 1f;
        transform.DOScale(originalScale * 1.5f, duration).SetLoops(-1, LoopType.Yoyo);
        sr.DOColor(Color.black, duration).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopAnimating()
    {
        transform.DOKill();
        transform.localScale = originalScale;

        sr.DOKill();
        sr.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!journeyMapManager.availableNodes.Contains(journeyNode))
            transform.DOScale(originalScale * 1.2f, 1f);

        journeyMapUI.ShowNodeInfo(journeyNode);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!journeyMapManager.availableNodes.Contains(journeyNode))
            transform.DOScale(originalScale, 1f);

        journeyMapUI.HideNodeInfo();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        journeyMapManager.NodeClick(this);
    }

    public void DrawCircle()
    {

        Animator anim = GetComponentInChildren<Animator>();
        anim.enabled = true;
    }

}

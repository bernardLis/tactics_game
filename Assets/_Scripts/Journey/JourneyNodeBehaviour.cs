using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class JourneyNodeBehaviour : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    JourneyMapManager _journeyMapManager;
    JourneyMapUI _journeyMapUI;

    [SerializeField] SpriteRenderer _visitedGFX;

    [HideInInspector] public JourneyNode JourneyNode;

    Vector3 _originalScale;
    SpriteRenderer _spriteRenderer;

    public void Initialize(JourneyNode _jn)
    {
        JourneyNode = _jn;
        _journeyMapManager = JourneyMapManager.Instance;
        _journeyMapUI = _journeyMapManager.GetComponent<JourneyMapUI>();

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _originalScale = transform.localScale;
    }

    public void AnimateAvailableNode()
    {
        float duration = 1f;
        transform.DOScale(_originalScale * 1.5f, duration).SetLoops(-1, LoopType.Yoyo);
        _spriteRenderer.DOColor(Color.black, duration).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopAnimating()
    {
        transform.DOKill();
        transform.localScale = _originalScale;

        _spriteRenderer.DOKill();
        _spriteRenderer.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_journeyMapManager.AvailableNodes.Contains(JourneyNode))
            transform.DOScale(_originalScale * 1.2f, 1f);

        _journeyMapUI.ShowNodeInfo(JourneyNode);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_journeyMapManager.AvailableNodes.Contains(JourneyNode))
            transform.DOScale(_originalScale, 1f);

        _journeyMapUI.HideNodeInfo();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _journeyMapManager.NodeClick(this);
    }

    public void DrawCircle()
    {
        Animator anim = GetComponentInChildren<Animator>();
        anim.enabled = true;
    }

    public void MarkAsVisited()
    {
        _spriteRenderer.color = Color.black;
        _visitedGFX.enabled = true;
    }

}

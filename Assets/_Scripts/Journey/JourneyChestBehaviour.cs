using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class JourneyChestBehaviour : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Unity Setup")]
    [SerializeField] GameObject _forbiddenMarker;

    JourneyChestManager _journeyChestManager;

    [HideInInspector] public bool WasChestSelected;
    [HideInInspector] public JourneyChest Chest;

    float _timeOn;
    float _timeOff;

    bool _isHovering;
    bool _locked;

    void Start()
    {
        _journeyChestManager = JourneyChestManager.Instance;
    }

    void Update()
    {
        if (_isHovering)
            _timeOn += Time.deltaTime;
        if (!_isHovering && _locked)
            _timeOff += Time.deltaTime;

        if (_timeOn > 0.5f && !_locked)
            ForbidChest();

        if (_timeOff > 0.5f && _locked)
            UnforbidChest();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (WasChestSelected)
            return;

        _timeOn = 0f;
        _timeOff = 0f;

        _isHovering = true;

        transform.DOScale(Vector3.one * 0.8f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (WasChestSelected)
            return;

        _timeOn = 0f;

        _timeOff = 0f;
        _isHovering = false;

        transform.DOScale(Vector3.one, 1f);
    }

    void ForbidChest()
    {
        if (WasChestSelected)
            return;

        _locked = true;
        _forbiddenMarker.SetActive(true);
        _journeyChestManager.AddForbiddenChest(this);
    }

    void UnforbidChest()
    {
        _locked = false;
        _forbiddenMarker.SetActive(false);
        _journeyChestManager.RemoveForbiddenChest(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_locked)
            return;
        
        if (WasChestSelected)
            return;

        transform.DOScale(Vector3.one * 1.2f, 2f);

        _journeyChestManager.ChestWasSelected(this);
    }

}

using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class JourneyChestBehaviour : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Unity Setup")]
    public GameObject forbiddenMarker;

    JourneyChestManager journeyChestManager;

    [HideInInspector] public bool wasChestSelected;
    [HideInInspector] public JourneyChest chest;

    float timeOn;
    float timeOff;

    bool isHovering;
    bool locked;

    void Start()
    {
        journeyChestManager = JourneyChestManager.instance;
    }

    void Update()
    {
        if (isHovering)
            timeOn += Time.deltaTime;
        if (!isHovering && locked)
            timeOff += Time.deltaTime;

        if (timeOn > 0.5f && !locked)
            ForbidChest();

        if (timeOff > 0.5f && locked)
            UnforbidChest();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (wasChestSelected)
            return;

        Debug.Log("on pointer enter");
        timeOn = 0f;
        timeOff = 0f;

        isHovering = true;

        transform.DOScale(Vector3.one * 0.8f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (wasChestSelected)
            return;


        timeOn = 0f;

        timeOff = 0f;
        isHovering = false;

        transform.DOScale(Vector3.one, 1f);
    }

    void ForbidChest()
    {
        if (wasChestSelected)
            return;

        locked = true;
        forbiddenMarker.SetActive(true);
        journeyChestManager.AddForbiddenChest(this);
    }

    void UnforbidChest()
    {
        locked = false;
        forbiddenMarker.SetActive(false);
        journeyChestManager.RemoveForbiddenChest(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (locked)
            return;
        
        if (wasChestSelected)
            return;

        transform.DOScale(Vector3.one * 1.2f, 2f);

        JourneyChestManager.instance.ChestWasSelected(this);
    }

}

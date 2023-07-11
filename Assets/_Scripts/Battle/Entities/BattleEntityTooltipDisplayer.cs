using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleEntityTooltipDisplayer : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    BattleEntity _battleEntity;
    BattleTooltipManager _tooltipManager;

    void Start()
    {
        _battleEntity = GetComponent<BattleEntity>();
        _tooltipManager = BattleTooltipManager.Instance;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _tooltipManager.DisplayTooltip(_battleEntity);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo(_battleEntity);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }

}


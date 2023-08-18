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
        if (!CanDisplayTooltip()) return;
        _tooltipManager.ShowTooltip(_battleEntity);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanDisplayTooltip()) return;
        _tooltipManager.ShowInfo(_battleEntity);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!CanDisplayTooltip()) return;
        _tooltipManager.HideInfo();
    }

    bool CanDisplayTooltip()
    {
        if (_tooltipManager == null) return false;
        if (_battleEntity.IsDead) return false;

        return true;
    }

}


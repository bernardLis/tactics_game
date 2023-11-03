using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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
        VisualElement el = new BattleEntityCard(_battleEntity);

        _tooltipManager.ShowTooltip(el, gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanDisplayTooltip()) return;
        _tooltipManager.ShowEntityInfo(_battleEntity);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!CanDisplayTooltip()) return;
        _tooltipManager.HideEntityInfo();
    }

    bool CanDisplayTooltip()
    {
        if (_tooltipManager == null) return false;
        if (_battleEntity.IsDead) return false;

        return true;
    }

}


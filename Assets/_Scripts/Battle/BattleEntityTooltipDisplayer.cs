using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleEntityTooltipDisplayer : MonoBehaviour, IPointerDownHandler//,IPointerEnterHandler, IPointerExitHandler
{
    BattleEntityTooltipManager _tooltipManager;

    void Start()
    {
        _tooltipManager = BattleEntityTooltipManager.Instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _tooltipManager.DisplayTooltip(GetComponent<BattleEntity>());
    }
}


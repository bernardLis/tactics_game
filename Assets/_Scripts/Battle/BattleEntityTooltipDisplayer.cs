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
        Debug.Log("On Pointer Enter");
        _tooltipManager.DisplayTooltip(GetComponent<BattleEntity>());
    }

    //   public void OnPointerEnter(PointerEventData eventData)
    //   {
    //      Debug.Log("On Pointer Enter");
    //       _tooltipManager.DisplayTooltip(GetComponent<BattleEntity>());
    //   }

    //   public void OnPointerExit(PointerEventData eventData)
    //   {
    //       Debug.Log("On Pointer Exit");
    //       _tooltipManager.HideTooltip();
    //   }


}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MapTooltipDisplayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //   FogOfWarObject _fogOfWarObject;
    [SerializeField] TextMeshProUGUI _tmpText;

    void Start()
    {
        //      _fogOfWarObject = GetComponent<FogOfWarObject>();
        _tmpText.enabled = false;
    }

    public void OnPointerEnter(PointerEventData evt) { DisplayTooltip(); }

    public void OnPointerExit(PointerEventData evt) { HideTooltip(); }

    public void DisplayTooltip()
    {
        // if (_fogOfWarObject != null && !_fogOfWarObject.IsCurrentlyVisible)
        //     return;

        if (TryGetComponent<ITooltipDisplayable>(out ITooltipDisplayable t))
        {
            _tmpText.enabled = true;
            _tmpText.text = t.GetTooltipText();
        }
    }

    public void HideTooltip() { _tmpText.enabled = false; }
}


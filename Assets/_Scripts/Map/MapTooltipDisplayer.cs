using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MapTooltipDisplayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] TextMeshProUGUI _tmpText;

    void Start()
    {
        _tmpText.enabled = false;
    }

    public void OnPointerEnter(PointerEventData evt)
    {
        Debug.Log($"on pointer enter");
        if (TryGetComponent<ITooltipDisplayable>(out ITooltipDisplayable t))
        {
            _tmpText.enabled = true;
            _tmpText.text = t.GetTooltipText();
        }
    }

    public void OnPointerExit(PointerEventData evt)
    {
        _tmpText.enabled = false;
    }
}


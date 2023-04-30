using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class BattleEntityInfoDisplayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject _info;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _info.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _info.SetActive(false);
    }
}

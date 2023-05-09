using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BattleGrabbableEntity : MonoBehaviour, IPointerDownHandler
{
    BattleGrabManager _grabManager;

    void Start()
    {
        _grabManager = BattleGrabManager.Instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        BattleEntity be = GetComponent<BattleEntity>();
        if (!be.CanBeGrabbed()) return;
        _grabManager.TryGrabbing(be);

    }
}

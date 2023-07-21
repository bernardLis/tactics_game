using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleGrabbableTurret : MonoBehaviour, IPointerDownHandler
{
    BattleGrabManager _grabManager;

    void Start()
    {
        _grabManager = BattleGrabManager.Instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        BattleTurret bt = GetComponent<BattleTurret>();
        _grabManager.TryGrabbing(bt);
    }

}

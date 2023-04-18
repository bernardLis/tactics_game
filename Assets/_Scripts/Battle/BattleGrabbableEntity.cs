using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BattleGrabbableEntity : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    BattleGrabManager _grabManager;

    bool _isPointerDown;
    void Start()
    {
        _grabManager = BattleGrabManager.Instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        _isPointerDown = true;
        StartCoroutine(Grab());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;
    }

    IEnumerator Grab()
    {
        yield return new WaitForSeconds(0.2f);
        if (!_isPointerDown) yield break;
        _grabManager.TryGrabbing(GetComponent<BattleEntity>());
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
public class JourneyCameraController : MonoBehaviour
{
    PlayerInput _playerInput;
    void Start()
    {
        _playerInput = JourneyMapManager.instance.GetComponent<PlayerInput>();
        UnsubscribeInputActions();
        SubscribeInputActions();
    }

    /* INPUT */
    void SubscribeInputActions()
    {
        _playerInput.actions["ArrowMovement"].performed += Move;
    }

    public void UnsubscribeInputActions()
    {
        _playerInput.actions["ArrowMovement"].performed -= Move;
    }

    // TODO: this should be better
    void Move(InputAction.CallbackContext _ctx)
    {
        Vector3 change = Vector3.one * _ctx.ReadValue<Vector2>() * 30f;
        Vector3 endPos = transform.position + change;
        transform.DOMove(endPos, 0.5f);
    }
}

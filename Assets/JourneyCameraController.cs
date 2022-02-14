using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
public class JourneyCameraController : MonoBehaviour
{
    PlayerInput playerInput;
    void Start()
    {
        playerInput = JourneyMapManager.instance.GetComponent<PlayerInput>();
        SubscribeInputActions();

    }

    /* INPUT */
    void OnEnable()
    {
    }

    void OnDisable()
    {
        if (playerInput == null)
            return;

        UnsubscribeInputActions();
    }

    void SubscribeInputActions()
    {
        playerInput.actions["ArrowMovement"].performed += ctx => Move(ctx.ReadValue<Vector2>());
    }

    void UnsubscribeInputActions()
    {
        playerInput.actions["ArrowMovement"].performed -= ctx => Move(ctx.ReadValue<Vector2>());
    }

    void Move(Vector2 _direction)
    {
        Vector3 change = Vector3.one * _direction * 30f;
        Vector3 endPos = transform.position + change;
        transform.DOMove(endPos, 0.5f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraArrowMovement : MonoBehaviour
{
    GameManager _gameManager;
    PlayerInput _playerInput;

    Vector2 _velocity;
    [SerializeField] float _speed;

    void OnEnable()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Map");
        UnsubscribeInputActions();
        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (_playerInput == null)
            return;

        UnsubscribeInputActions();
    }

    void SubscribeInputActions()
    {
        _playerInput.actions["ArrowMovement"].performed += ArrowMovement;
        _playerInput.actions["ArrowMovement"].canceled += ResetVelocity;

    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["ArrowMovement"].performed -= ArrowMovement;
        _playerInput.actions["ArrowMovement"].canceled += ResetVelocity;
    }

    void ArrowMovement(InputAction.CallbackContext ctx) { _velocity = ctx.ReadValue<Vector2>(); }

    void ResetVelocity(InputAction.CallbackContext ctx) { _velocity = Vector2.zero; }

    void LateUpdate() { transform.Translate(_velocity * _speed * Time.fixedDeltaTime); }


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CameraTarget : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;
    PlayerInput _playerInput;

    [SerializeField] Transform _cameraTransform;


    Vector3 _movementDirection;
    bool _disableUpdate;
    public float _moveSpeed = 10f;

    [Header("Vertical Motion - zooming")]
    [SerializeField] float _stepSize = 1f;
    [SerializeField] float _minHeight = 0f;
    [SerializeField] float _maxHeight = 20f;
    [SerializeField] float _defaultZoomHeight = 0f;
    float _zoomHeight;

    [Header("Rotation")]
    [SerializeField] float _maxRotationSpeed = 0.1f;

    public event Action OnCameraMoved;
    public event Action OnCameraRotated;

    void Start()
    {
        _battleManager = BattleManager.Instance;
        // _battleManager.OnGamePaused += () => _disableUpdate = true;
        // _battleManager.OnGameResumed += () => StartCoroutine(DelayedStart());
    }

    // IEnumerator DelayedStart()
    // {
    //     yield return new WaitForSeconds(0.1f);
    //     _disableUpdate = false;
    // }

    // void Update()
    // {

    //     GetKeyboardMovement();
    //     // CheckMouseAtScreenEdge();

    //     // UpdateVelocity();
    //     // UpdateCameraPosition();
    //     // UpdateBasePosition();
    // }

    /* INPUT */
    void OnEnable()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Battle");
        UnsubscribeInputActions();
        SubscribeInputActions();

        // _zoomHeight = _cameraTransform.localPosition.y;
        // _lastPosition = transform.position;
        // _movement = _playerInput.actions["CameraMovement"];
    }

    void OnDisable()
    {
        if (_playerInput == null) return;

        UnsubscribeInputActions();
    }

    void OnDestroy()
    {
        if (_playerInput == null) return;

        UnsubscribeInputActions();
    }

    void SubscribeInputActions()
    {
        _playerInput.actions["CameraMovement"].performed += CameraMovement;
        _playerInput.actions["CameraMovement"].canceled += StopCameraMovement;

        _playerInput.actions["RotateCamera"].performed += RotateCamera;
        _playerInput.actions["ZoomCamera"].performed += ZoomCamera;
        // _playerInput.actions["CameraDefaultPosition"].performed += MoveCameraToDefaultPosition;
        _playerInput.actions["RotateCameraLeft"].performed += RotateCameraLeft;
        _playerInput.actions["RotateCameraRight"].performed += RotateCameraRight;

    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["CameraMovement"].performed -= CameraMovement;
        _playerInput.actions["CameraMovement"].canceled -= StopCameraMovement;

        _playerInput.actions["RotateCamera"].performed -= RotateCamera;
        _playerInput.actions["ZoomCamera"].performed -= ZoomCamera;
        // _playerInput.actions["CameraDefaultPosition"].performed -= MoveCameraToDefaultPosition;
        _playerInput.actions["RotateCameraLeft"].performed -= RotateCameraLeft;
        _playerInput.actions["RotateCameraRight"].performed -= RotateCameraRight;
    }

    void CameraMovement(InputAction.CallbackContext context)
    {


        Vector3 inputValue = context.ReadValue<Vector2>();
        inputValue = inputValue.normalized;
        Debug.Log($"inputValue {inputValue}");

        _movementDirection = new Vector3(-inputValue.x, 0, -inputValue.y);

        // if (inputValue.sqrMagnitude > 0.1f)
        //     transform.position += inputValue;
    }

    void StopCameraMovement(InputAction.CallbackContext context)
    {
        _movementDirection = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (_disableUpdate) return;
        transform.Translate(_movementDirection * _moveSpeed * Time.fixedDeltaTime);
    }

    void RotateCamera(InputAction.CallbackContext ctx)
    {
        if (this == null) return;

        if (!Mouse.current.middleButton.isPressed) return;

        Debug.Log($"rotate camera");

        float value = ctx.ReadValue<Vector2>().x;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, value * _maxRotationSpeed + transform.eulerAngles.y, 0);
        OnCameraRotated?.Invoke();
    }

    void RotateCameraLeft(InputAction.CallbackContext ctx)
    {
        if (this == null) return;
        transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 30f, 0f), 0.3f);
        OnCameraRotated?.Invoke();
    }

    void RotateCameraRight(InputAction.CallbackContext ctx)
    {
        if (this == null) return;
        transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 30f, 0f), 0.3f);
        OnCameraRotated?.Invoke();
    }

    void ZoomCamera(InputAction.CallbackContext ctx)
    {
        if (this == null) return;

        float value = -ctx.ReadValue<Vector2>().y / 100f; // I prefer to scroll to myself to zoom out
        if (Mathf.Abs(value) < 0.01f) return;

        _zoomHeight = transform.localPosition.y + value * _stepSize;
        _zoomHeight = Mathf.Clamp(_zoomHeight, _minHeight, _maxHeight);

        Debug.Log($"zoom height {_zoomHeight}");
        transform.DOLocalMoveY(_zoomHeight, 0.3f);
    }



}

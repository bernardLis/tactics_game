using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// https://www.youtube.com/watch?v=3Y7TFN_DsoI
public class BattleCameraManager : MonoBehaviour
{
    GameManager _gameManager;
    PlayerInput _playerInput;

    InputAction _movement;
    Transform _cameraTransform;

    [Header("Horizontal Motion")]
    [SerializeField] float _maxSpeed = 5f;
    float _speed;
    [SerializeField] float _acceleration = 10f;
    [SerializeField] float _damping = 15f;

    [Header("Vertical Motion - zooming")]
    [SerializeField] float _stepSize = 2f;
    [SerializeField] float _zoomDampening = 7.5f;
    [SerializeField] float _minHeight = 5f;
    [SerializeField] float _maxHeight = 50f;
    [SerializeField] float _zoomSpeed = 2f;

    [Header("Rotation")]
    [SerializeField] float _maxRotationSpeed = 0.5f;

    [Header("Screen Edge Motion")]
    [SerializeField][Range(0f, 0.1f)] float _edgeTolerance = 0.05f;
    [SerializeField] bool _useScreenEdge = true;

    float _zoomHeight;
    Vector3 _targetPosition;
    Vector3 _horizontalVelocity;
    Vector3 _lastPosition;

    Vector3 _startDrag;

    void Awake()
    {
        _cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        GetKeyboardMovement();

        UpdateVelocity();
        UpdateCameraPosition();
        UpdateBasePosition();
    }

    /* INPUT */
    void OnEnable()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Battle");
        UnsubscribeInputActions();
        SubscribeInputActions();

        _zoomHeight = _cameraTransform.localPosition.y;
        _lastPosition = transform.position;
        _movement = _playerInput.actions["CameraMovement"];
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

        _playerInput.actions["RotateCamera"].performed += RotateCamera;
        _playerInput.actions["ZoomCamera"].performed += ZoomCamera;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["RotateCamera"].performed -= RotateCamera;
        _playerInput.actions["ZoomCamera"].performed -= ZoomCamera;

    }

    void UpdateVelocity()
    {
        _horizontalVelocity = (transform.position - _lastPosition) / Time.deltaTime;
        _horizontalVelocity.y = 0;
        _lastPosition = transform.position;
    }

    void GetKeyboardMovement()
    {
        Vector3 inputValue = _movement.ReadValue<Vector2>().x * GetCameraRight()
            + _movement.ReadValue<Vector2>().y * GetCameraForward();

        inputValue = inputValue.normalized;

        if (inputValue.sqrMagnitude > 0.1f)
            _targetPosition += inputValue;

    }

    Vector3 GetCameraRight()
    {
        Vector3 v = _cameraTransform.right;
        v.y = 0; // keep movement in horizontal plane
        return v;
    }

    Vector3 GetCameraForward()
    {
        Vector3 v = _cameraTransform.forward;
        v.y = 0; // keep movement in horizontal plane
        return v;
    }

    void UpdateBasePosition()
    {
        if (_targetPosition.sqrMagnitude > 0.1f)
        {
            _speed = Mathf.Lerp(_speed, _maxSpeed, _acceleration * Time.deltaTime);
            transform.position += _targetPosition * _speed * Time.deltaTime;
        }
        else // slow down
        {
            _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, Vector3.zero, _damping * Time.deltaTime);
            transform.position += _horizontalVelocity * Time.deltaTime;
        }

        _targetPosition = Vector3.zero;
    }

    void RotateCamera(InputAction.CallbackContext ctx)
    {
        if (!Mouse.current.middleButton.isPressed) return;

        float value = ctx.ReadValue<Vector2>().x;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, value * _maxRotationSpeed + transform.eulerAngles.y, 0);
    }

    void ZoomCamera(InputAction.CallbackContext ctx)
    {
        float value = -ctx.ReadValue<Vector2>().y / 100f; // I prefer to scroll to myself to zoom out
        if (Mathf.Abs(value) < 0.01f) return;

        _zoomHeight = _cameraTransform.localPosition.y + value * _stepSize;
        _zoomHeight = Mathf.Clamp(_zoomHeight, _minHeight, _maxHeight);
    }

    void UpdateCameraPosition()
    {
        Vector3 zoomTarget = new(_cameraTransform.localPosition.x, _zoomHeight, _cameraTransform.localPosition.z);
        zoomTarget -= _zoomSpeed * (_zoomHeight - _cameraTransform.localPosition.y) * Vector3.forward;

        _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, zoomTarget, _zoomDampening * Time.deltaTime);
        //_cameraTransform.LookAt(transform.position);
    }
}

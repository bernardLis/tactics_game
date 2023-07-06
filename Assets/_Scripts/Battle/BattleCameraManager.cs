using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

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
    [SerializeField] float _defaultZoomHeight = 8f;


    [Header("Rotation")]
    [SerializeField] float _maxRotationSpeed = 0.5f;

    [Header("Screen Edge Motion")]
    [SerializeField][Range(0f, 0.1f)] float _edgeTolerance = 0.05f;
    [SerializeField] bool _useScreenEdge = true;

    float _zoomHeight;

    Vector3 _targetPosition;
    Vector3 _horizontalVelocity;
    Vector3 _lastPosition;

    bool _disableUpdate;

    void Awake()
    {
        _cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        if (_disableUpdate) return;

        GetKeyboardMovement();
        CheckMouseAtScreenEdge();

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
        _playerInput.actions["CameraDefaultPosition"].performed += MoveCameraToDefaultPosition;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["RotateCamera"].performed -= RotateCamera;
        _playerInput.actions["ZoomCamera"].performed -= ZoomCamera;
        _playerInput.actions["CameraDefaultPosition"].performed -= MoveCameraToDefaultPosition;
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
        if (this == null) return;

        if (!Mouse.current.middleButton.isPressed) return;

        float value = ctx.ReadValue<Vector2>().x;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, value * _maxRotationSpeed + transform.eulerAngles.y, 0);
    }

    void ZoomCamera(InputAction.CallbackContext ctx)
    {
        if (this == null) return;

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

    void CheckMouseAtScreenEdge()
    {
        if (!_useScreenEdge) return;
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 moveDirection = Vector3.zero;

        if (mousePosition.x < _edgeTolerance * Screen.width)
            moveDirection += -GetCameraRight();
        else if (mousePosition.x > (1f - _edgeTolerance) * Screen.width)
            moveDirection += GetCameraRight();

        if (mousePosition.y < _edgeTolerance * Screen.height)
            moveDirection += -GetCameraForward();
        else if (mousePosition.y > (1f - _edgeTolerance) * Screen.height)
            moveDirection += GetCameraForward();

        _targetPosition += moveDirection;
    }

    void MoveCameraToDefaultPosition(InputAction.CallbackContext ctx)
    {
        if (this == null) return;

        transform.DOMove(new Vector3(24f, 0f, -6f), 0.5f);
        transform.DORotate(new Vector3(30f, -90f, 0f), 0.5f);
        _zoomHeight = _defaultZoomHeight;
    }

    public void MoveCameraToDefaultPosition(float time)
    {
        if (this == null) return;

        _disableUpdate = true;

        transform.DOMove(new Vector3(24f, 0f, -6f), time);
        transform.DORotate(new Vector3(30f, -90f, 0f), time);

        _cameraTransform.DOLocalMoveY(_defaultZoomHeight, time)
                .OnComplete(() =>
                {
                    _zoomHeight = _cameraTransform.localPosition.y;
                    _lastPosition = transform.position;

                    _disableUpdate = false;
                });

    }

    public void MoveCameraTo(Vector3 position, Vector3 rotation, float zoomHeight)
    {
        transform.DOMove(position, 0.5f);
        transform.DORotate(rotation, 0.5f);
        _zoomHeight = zoomHeight;
    }

    public void CenterCameraOnBattleEntity(BattleEntity be)
    {
        Vector3 pos = be.transform.forward * -10f + be.transform.position;
        transform.DOMove(pos, 0.5f);

        Vector3 rot = new Vector3(30, be.transform.localEulerAngles.y, 0f);
        transform.DORotate(rot, 0.5f);
        
        _zoomHeight = _defaultZoomHeight;
    }
}

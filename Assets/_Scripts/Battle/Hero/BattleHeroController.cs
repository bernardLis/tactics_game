using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

/* BASED ON UNITY'S STARTER ASSET */
public class BattleHeroController : MonoBehaviour
{

    [Header("Player Movement")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.3f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Zoom")]
    [SerializeField] CinemachineFollowZoom _cinemachineFollowZoom;
    [SerializeField] float _stepSize = 1f;
    [SerializeField] float _minZoom = 5f;
    [SerializeField] float _maxZoom = 25f;

    [Header("Camera Rotation")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    [SerializeField] GameObject _cinemachineCameraTarget;
    Vector3 _defaultCameraRotation = new Vector3(50f, 0f, 0f);
    float _threshold = 0.01f;
    float _cinemachineTargetYaw;
    float _cinemachineTargetPitch;

    GameManager _gameManager;
    PlayerInput _playerInput;
    BattleManager _battleManager;
    BattleFightManager _battleFightManager;
    BattleInteractor _battleInteractor;

    Animator _animator;
    CharacterController _controller;

    Vector3 _movementDirection;
    bool _disableUpdate;

    // player
    float _speed;
    float _animationBlend;
    float _targetRotation = 0.0f;
    float _rotationVelocity;

    // animation IDs
    int _animVelocityX;
    int _animVelocityZ;

    // HERE: testing
    bool _isSprinting;

    // strafe
    int _strafeInput;
    float _strafeAnimationBlend;
    float _strafeSpeed;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _battleManager = BattleManager.Instance;
        _battleManager.OnGamePaused += () => _disableUpdate = true;
        _battleManager.OnGameResumed += () => StartCoroutine(DelayedStart(0.1f));

        _animator = GetComponentInChildren<Animator>();
        _controller = GetComponent<CharacterController>();

        _cinemachineTargetPitch = _defaultCameraRotation.x;
        _cinemachineTargetYaw = _defaultCameraRotation.y;

        AssignAnimationIDs();
    }

    IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        _disableUpdate = false;
    }

    void Update()
    {
        if (_disableUpdate) return;
        Move();
        RotatePlayer();
        Strafe();
    }

    void LateUpdate()
    {
        if (_disableUpdate) return;
        RotateCamera();
    }

    void AssignAnimationIDs()
    {
        _animVelocityX = Animator.StringToHash("VelocityX");
        _animVelocityZ = Animator.StringToHash("VelocityZ");
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
        _playerInput.actions["StrafeLeft"].performed += StrafeLeft;
        _playerInput.actions["StrafeLeft"].canceled += ResetStrafe;

        _playerInput.actions["StrafeRight"].performed += StrafeRight;
        _playerInput.actions["StrafeRight"].canceled += ResetStrafe;

        _playerInput.actions["Shift"].performed += SetSprinting;
        _playerInput.actions["Shift"].canceled += ResetSprinting;

        _playerInput.actions["PlayerMovement"].performed += GetMovementVector;
        _playerInput.actions["PlayerMovement"].canceled += ResetMovementVector;

        _playerInput.actions["ZoomCamera"].performed += ZoomCamera;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["StrafeLeft"].performed -= StrafeLeft;
        _playerInput.actions["StrafeLeft"].canceled -= ResetStrafe;

        _playerInput.actions["StrafeRight"].performed -= StrafeRight;
        _playerInput.actions["StrafeRight"].canceled -= ResetStrafe;

        _playerInput.actions["Shift"].performed -= SetSprinting;
        _playerInput.actions["Shift"].canceled -= ResetSprinting;

        _playerInput.actions["PlayerMovement"].performed -= GetMovementVector;
        _playerInput.actions["PlayerMovement"].canceled -= ResetMovementVector;

        _playerInput.actions["ZoomCamera"].performed -= ZoomCamera;
    }

    /* STRAFE */
    void Strafe()
    {
        if (_speed != 0) return;
        if (_strafeInput == 0) return;

        _strafeSpeed = MoveSpeed * 0.5f;

        // animation
        _strafeAnimationBlend = Mathf.Lerp(_strafeAnimationBlend, _strafeSpeed, Time.deltaTime * SpeedChangeRate);
        if (_strafeAnimationBlend < 0.01f) _strafeAnimationBlend = 0f;
        _animator.SetFloat(_animVelocityX, _strafeInput * _strafeAnimationBlend);

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f)
                                * Vector3.right
                                * _strafeInput;
        targetDirection.y = -0.01f;
        _controller.Move(targetDirection.normalized * (_strafeSpeed * Time.deltaTime));
    }

    void StrafeLeft(InputAction.CallbackContext ctx)
    {
        if (this == null) return;
        _strafeInput = -1;
    }

    void StrafeRight(InputAction.CallbackContext ctx)
    {
        if (this == null) return;
        _strafeInput = 1;
    }

    void ResetStrafe(InputAction.CallbackContext ctx)
    {
        if (this == null) return;
        _strafeInput = 0;
        _strafeSpeed = 0;
        _animator.SetFloat(_animVelocityX, 0);
    }

    void SetSprinting(InputAction.CallbackContext ctx)
    {
        _isSprinting = true;
    }

    void ResetSprinting(InputAction.CallbackContext ctx)
    {
        _isSprinting = false;
    }

    /* MOVE */
    void GetMovementVector(InputAction.CallbackContext context)
    {
        Vector3 inputValue = context.ReadValue<Vector2>();
        inputValue = inputValue.normalized;

        _movementDirection = new Vector3(inputValue.x, 0, inputValue.y);
    }

    void ResetMovementVector(InputAction.CallbackContext context)
    {
        _movementDirection = Vector3.zero;

        // recenter transform, idk if it is a good idea but it's here just in case... xD
        if (_animator == null) return;
        _animator.transform.DOKill();
        _animator.transform.DOLocalMove(Vector3.zero, 1f);
    }

    public void SetMoveSpeed(int speed)
    {
        MoveSpeed = speed;
    }

    void Move()
    {
        if (!IsGrounded()) return;
        if (_strafeSpeed != 0) return;

        float targetSpeed = MoveSpeed;
        if (_isSprinting) targetSpeed *= 2f;
        if (_movementDirection.z < 0) targetSpeed *= 0.5f;
        if (_movementDirection.z == 0) targetSpeed = 0.0f;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f,
                                            _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _movementDirection.magnitude;
        _speed = targetSpeed;
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                                Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f; // round speed to 3 decimal places
        }

        Vector3 inputDirection = _movementDirection.normalized;

        // move
        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f)
                                * Vector3.forward
                                * inputDirection.z;
        targetDirection.y = -0.01f;
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime));

        // animation
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        _animator.SetFloat(_animVelocityZ, inputDirection.z * _animationBlend);
        _animator.SetFloat(_animVelocityX, inputDirection.x * _animationBlend);
        if (inputDirection.z <= 0) _animator.SetFloat(_animVelocityX, 0); // walking backwards looks bad when blended
    }

    bool IsGrounded()
    {
        return transform.position.y < 0.1f;
    }

    /* ROTATE */
    void RotatePlayer()
    {
        if (_movementDirection == Vector3.zero) return;
        Vector3 inputDirection = _movementDirection.normalized;

        _targetRotation = transform.eulerAngles.y + 30 * inputDirection.x;

        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y,
                                        _targetRotation,
                                        ref _rotationVelocity,
                                        RotationSmoothTime);
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }

    /* CAMERA */
    void ZoomCamera(InputAction.CallbackContext context)
    {
        if (this == null) return;

        float value = -context.ReadValue<Vector2>().y / 100f;
        if (Mathf.Abs(value) < 0.01f) return;

        float zoomValue = _cinemachineFollowZoom.m_Width + value * _stepSize;
        _cinemachineFollowZoom.m_Width = Mathf.Clamp(zoomValue,
                 _minZoom, _maxZoom);
    }

    void RotateCamera()
    {
        //TODO: camera rotation is shit
        if (this == null) return;
        if (!Mouse.current.middleButton.isPressed) return;
        _cinemachineCameraTarget.transform.DOKill();
        _cinemachineCameraTarget.transform.DOLocalRotate(_defaultCameraRotation, 1f)
            .SetEase(Ease.InOutSine)
            .SetDelay(0.5f)
            .OnComplete(() =>
            {
                _cinemachineTargetPitch = _defaultCameraRotation.x;
                _cinemachineTargetYaw = _defaultCameraRotation.y;
            });

        Vector2 value = _playerInput.actions["RotateCamera"].ReadValue<Vector2>();

        if (value.sqrMagnitude < _threshold) return;

        _cinemachineTargetPitch += value.y;
        _cinemachineTargetYaw += value.x;

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, float.MinValue, float.MaxValue);
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);

        _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,
                                                     _cinemachineTargetYaw, 0);
    }

    float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

}

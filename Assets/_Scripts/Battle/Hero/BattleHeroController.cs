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


    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

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

    [Header("Audio")]
    [SerializeField] Sound _footstepSound;

    GameManager _gameManager;
    AudioManager _audioManager;
    BattleManager _battleManager;
    PlayerInput _playerInput;

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
    int _animIDSpeed;
    int _animIDGrounded;
    int _animIDJump;
    int _animIDFreeFall;
    int _animIDMotionSpeed;

    private bool _hasAnimator;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;

        _battleManager = BattleManager.Instance;
        _battleManager.OnGamePaused += () => _disableUpdate = true;
        _battleManager.OnGameResumed += () => StartCoroutine(DelayedStart());

        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();

        _cinemachineTargetPitch = _defaultCameraRotation.x;
        _cinemachineTargetYaw = _defaultCameraRotation.y;

        AssignAnimationIDs();
    }

    void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.1f);
        _disableUpdate = false;
    }

    void Update()
    {
        if (_disableUpdate) return;
        Move();
        RotatePlayer();
    }

    void LateUpdate()
    {
        if (_disableUpdate) return;
        RotateCamera();
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
        _playerInput.actions["PlayerMovement"].performed += GetMovementVector;
        _playerInput.actions["PlayerMovement"].canceled += ResetMovementVector;

        _playerInput.actions["ZoomCamera"].performed += ZoomCamera;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["PlayerMovement"].performed -= GetMovementVector;
        _playerInput.actions["PlayerMovement"].canceled -= ResetMovementVector;

        _playerInput.actions["ZoomCamera"].performed -= ZoomCamera;
    }

    void GetMovementVector(InputAction.CallbackContext context)
    {
        Vector3 inputValue = context.ReadValue<Vector2>();
        inputValue = inputValue.normalized;

        _movementDirection = new Vector3(inputValue.x, 0, inputValue.y);
    }

    void ResetMovementVector(InputAction.CallbackContext context)
    {
        _movementDirection = Vector3.zero;
    }

    public void SetMoveSpeed(int speed)
    {
        MoveSpeed = speed;
    }

    void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon
        // if there is no input, set the target speed to 0
        if (_movementDirection.z == 0) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f,
                                            _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _movementDirection.magnitude;//_input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        _speed = targetSpeed;
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                                Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f; // round speed to 3 decimal places
        }

        Vector3 inputDirection = _movementDirection.normalized;
        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f)
                               * Vector3.forward
                               * inputDirection.z;
        targetDirection.y = -0.01f;
        // update animator if using character
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        float motionSpeed = inputDirection.z == 0 ? 0 : 1;
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, motionSpeed);
        }

        if (_speed == 0) return;
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime));
    }

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


    void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
            if (_footstepSound != null)
                _audioManager.PlaySFX(_footstepSound, transform.TransformPoint(_controller.center));
    }

    // not used coz no jumps
    void OnLand(AnimationEvent animationEvent)
    {
    }

    float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

}

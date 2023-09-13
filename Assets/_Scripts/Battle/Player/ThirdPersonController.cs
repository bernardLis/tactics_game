using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/* BASED ON UNITY'S STARTER ASSET */
public class ThirdPersonController : MonoBehaviour
{

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.3f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

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
    [SerializeField] float _stepSize = 1f;
    [SerializeField] float _minHeight = 0f;
    [SerializeField] float _maxHeight = 20f;
    [SerializeField] float _defaultZoomHeight = 0f;

    GameManager _gameManager;
    BattleManager _battleManager;
    PlayerInput _playerInput;

    private Animator _animator;
    private CharacterController _controller;

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


    // camera rotation
    float _threshold = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private bool _hasAnimator;

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnGamePaused += () => _disableUpdate = true;
        _battleManager.OnGameResumed += () => StartCoroutine(DelayedStart());

        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();

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

    void FixedUpdate()
    {
        if (_disableUpdate) return;
        Move();
        RotatePlayer();
        GroundedCheck();
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

    void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }


    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = MoveSpeed; //_input.sprint ? SprintSpeed : MoveSpeed;

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

        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime));

        // update animator if using character
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        float motionSpeed = inputDirection.z == 0 ? 0 : 1;
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, motionSpeed);
        }

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

        // float value = -ctx.ReadValue<Vector2>().y / 100f; // I prefer to scroll to myself to zoom out
        // if (Mathf.Abs(value) < 0.01f) return;

        // _zoomHeight = transform.localPosition.y + value * _stepSize;
        // _zoomHeight = Mathf.Clamp(_zoomHeight, _minHeight, _maxHeight);

        // transform.DOLocalMoveY(_zoomHeight, 0.3f);
    }


    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            // if (FootstepAudioClips.Length > 0)
            // {
            //     var index = Random.Range(0, FootstepAudioClips.Length);
            //     AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            // }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            // AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }


}

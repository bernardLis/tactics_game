using System.Collections;
using Cinemachine;
using DG.Tweening;
using Lis.Battle;
using Lis.Core;
using UnityEngine;
using UnityEngine.InputSystem;

/* BASED ON UNITY'S STARTER ASSET */
namespace Lis.Units.Hero
{
    public class MovementController : MonoBehaviour
    {
        Camera _cam;
        Mouse _mouse;
        CinemachineVirtualCamera _cinemachineVirtualCamera;

        [Header("Player Movement")] [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        GameManager _gameManager;
        PlayerInput _playerInput;
        BattleManager _battleManager;

        Animator _animator;
        CharacterController _controller;

        Vector3 _movementDirection;
        bool _disableUpdate;

        // player
        bool _isSprintUnlocked;
        float _speed;
        float _animationBlend;
        float _targetRotation;
        float _rotationVelocity;

        // camera zoom
        readonly float _zoomSpeed = 0.05f;
        float _targetZoom;
        readonly float _zoomMin = 20f;
        readonly float _zoomMax = 60f;

        // animation IDs
        int _animVelocityX;
        int _animVelocityZ;

        // HERE: testing
        bool _isSprinting;

        // strafe
        private static readonly int Interaction = Animator.StringToHash("Interaction");

        void Start()
        {
            _cam = Camera.main;
            _mouse = Mouse.current;
            _gameManager = GameManager.Instance;

            _battleManager = BattleManager.Instance;
            _battleManager.OnGamePaused += () => _disableUpdate = true;
            _battleManager.OnGameResumed += () => StartCoroutine(DelayedStart(0.1f));

            _cinemachineVirtualCamera = _battleManager.GetComponent<HeroManager>().HeroFollowCamera;
            _targetZoom = _cinemachineVirtualCamera.m_Lens.FieldOfView;

            _animator = GetComponentInChildren<Animator>();
            _controller = GetComponent<CharacterController>();

            GetComponent<Interactor>().OnInteract += InteractionAnimation;

            AssignAnimationIDs();

            _isSprintUnlocked = _gameManager.UpgradeBoard.GetUpgradeByName("Hero Sprint").CurrentLevel != -1;
        }

        // TODO: this is wrong. It should somehow work without this code, but I don't know which button to click in Unity. 
        void InteractionAnimation()
        {
            _animator.SetLayerWeight(1, 1);
            _animator.SetTrigger(Interaction);
            Invoke(nameof(ResetArmsLayerWeight), 2f);
        }

        void ResetArmsLayerWeight()
        {
            _animator.SetLayerWeight(1, 0);
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
            SetAnimationBlend();
        }

        void LateUpdate()
        {
            if (_disableUpdate) return;

            RotateTowardsMouse();
            ZoomCameraSmoothly();

            // keeping player grounded
            Transform t = transform;
            Vector3 position = t.position;
            position = new(position.x, -0.035f, position.z);
            t.position = position;
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
            _playerInput.actions["Shift"].performed += SetSprinting;
            _playerInput.actions["Shift"].canceled += ResetSprinting;

            _playerInput.actions["PlayerMovement"].performed += GetMovementVector;
            _playerInput.actions["PlayerMovement"].canceled += ResetMovementVector;

            _playerInput.actions["ZoomCamera"].performed += ZoomCamera;
        }

        void UnsubscribeInputActions()
        {
            _playerInput.actions["Shift"].performed -= SetSprinting;
            _playerInput.actions["Shift"].canceled -= ResetSprinting;

            _playerInput.actions["PlayerMovement"].performed -= GetMovementVector;
            _playerInput.actions["PlayerMovement"].canceled -= ResetMovementVector;

            _playerInput.actions["ZoomCamera"].performed -= ZoomCamera;
        }

        void SetSprinting(InputAction.CallbackContext ctx)
        {
            if (!_isSprintUnlocked) return;

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

        public void SetMoveSpeed(float speed)
        {
            MoveSpeed = speed;
        }

        void Move()
        {
            if (!IsGrounded()) return;

            float targetSpeed = MoveSpeed;
            if (_isSprinting) targetSpeed *= 2f;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon
            Vector3 velocity = _controller.velocity;
            float currentHorizontalSpeed = new Vector3(velocity.x, 0.0f,
                velocity.z).magnitude;

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
            Vector3 targetDirection = inputDirection;
            targetDirection.y = -0.01f;
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime));
        }

        bool IsGrounded()
        {
            return transform.position.y < 0.1f;
        }

        void SetAnimationBlend()
        {
            _animationBlend = Mathf.Lerp(_animationBlend, _speed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = _movementDirection.normalized;
            Transform t = transform;
            Vector3 forward = t.forward;
            Vector3 right = t.right;
            float forwardDot = Vector3.Dot(inputDirection, forward);
            float rightDot = Vector3.Dot(inputDirection, right);
            float forwardBlend = Mathf.Abs(forwardDot);
            float rightBlend = Mathf.Abs(rightDot);
            float blend = Mathf.Max(forwardBlend, rightBlend);
            _animator.SetFloat(_animVelocityZ, forwardDot * blend * _animationBlend);
            _animator.SetFloat(_animVelocityX, rightDot * blend * _animationBlend);
        }

        void RotateTowardsMouse()
        {
            Vector3 mousePosition = _mouse.position.ReadValue();
            Ray ray = _cam.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 100, 1 << LayerMask.NameToLayer("Floor")))
                return;

            Vector3 relativePos = hit.point - transform.position;
            if (relativePos.magnitude < 0.1f)
                return; // HERE: does it help with jiggle when mouse is super close to player

            Quaternion lookRotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = Quaternion.Euler(new(0, lookRotation.eulerAngles.y, 0));
        }

        void ZoomCamera(InputAction.CallbackContext ctx)
        {
            // check if mouse is over UI
            if (!_battleManager.IsTimerOn) return;
            // so it is 0,120 and 0,-120 on mouse scroll
            Vector2 scrollValue = ctx.ReadValue<Vector2>();

            float newValue = _cinemachineVirtualCamera.m_Lens.FieldOfView - scrollValue.y * _zoomSpeed;
            _targetZoom = Mathf.Clamp(newValue, _zoomMin, _zoomMax);
        }

        void ZoomCameraSmoothly()
        {
            float newValue = Mathf.Lerp(_cinemachineVirtualCamera.m_Lens.FieldOfView, _targetZoom,
                Time.deltaTime * 5);
            _cinemachineVirtualCamera.m_Lens.FieldOfView = newValue;
        }
    }
}
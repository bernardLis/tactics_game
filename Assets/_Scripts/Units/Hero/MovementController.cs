using System.Collections;
using Cinemachine;
using DG.Tweening;
using Lis.Battle;
using Lis.Battle.Fight;
using Lis.Core;
using UnityEngine;
using UnityEngine.InputSystem;

/* BASED ON UNITY'S STARTER ASSET */
namespace Lis.Units.Hero
{
    public class MovementController : MonoBehaviour
    {
        [Header("Player Movement")] [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        readonly float _gravityValue = -9.81f;
        readonly float _zoomMax = 60f;
        readonly float _zoomMin = 20f;

        // camera zoom
        readonly float _zoomSpeed = 0.05f;
        float _animationBlend;

        Animator _animator;

        // animation IDs
        int _animVelocityX;
        int _animVelocityZ;
        BattleManager _battleManager;
        Camera _cam;
        CinemachineVirtualCamera _cinemachineVirtualCamera;
        CharacterController _controller;
        bool _disableUpdate;

        GameManager _gameManager;
        Transform _gfx;
        bool _groundedPlayer;

        Vector3 _inputDirection;

        // HERE: testing
        bool _isSprinting;

        // player
        bool _isSprintUnlocked;
        Mouse _mouse;
        PlayerInput _playerInput;

        Vector3 _playerVelocity;
        float _rotationVelocity;
        float _speed;
        float _targetRotation;
        float _targetZoom;

        Stat _maxStamina;
        FloatVariable _currentStamina;
        bool _staminaDepleted;
        readonly float _staminaDepletionRate = 1f;
        readonly float _staminaRefillRate = 1.3f;

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
            _gfx = _animator.transform;

            AssignAnimationIDs();

            _isSprintUnlocked = true; //_gameManager.UpgradeBoard.GetUpgradeByName("Hero Sprint").CurrentLevel != -1;
        }

        public void Initialize(Hero hero)
        {
            SetMoveSpeed(hero.Speed.GetValue());
            hero.Speed.OnValueChanged += SetMoveSpeed;

            _maxStamina = hero.MaxStamina;
            _currentStamina = hero.CurrentStamina;
        }

        void Update()
        {
            if (_disableUpdate) return;
            SetAnimationBlend();
            Move();
        }

        void LateUpdate()
        {
            if (_disableUpdate) return;

            RotateTowardsMouse();
            ZoomCameraSmoothly();
            RefillStamina();
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

        IEnumerator DelayedStart(float delay)
        {
            yield return new WaitForSeconds(delay);
            _disableUpdate = false;
        }

        public void DisableMovement()
        {
            _disableUpdate = true;
            _inputDirection = Vector3.zero;
            _animator.SetFloat(_animVelocityZ, 0);
            _animator.SetFloat(_animVelocityX, 0);
        }

        public void EnableMovement()
        {
            _disableUpdate = false;
        }

        void Move()
        {
            if (_currentStamina.Value >= _maxStamina.GetValue()) _staminaDepleted = false; // idk where to put it

            float targetSpeed = MoveSpeed;
            if (CanSprint() || !FightManager.IsFightActive) targetSpeed *= 1.5f;
            SetSpeed(targetSpeed);

            Vector3 moveDir = _inputDirection.normalized;
            moveDir.y = GetGravity();

            DepleteStamina();
            _controller.Move(moveDir * (_speed * Time.deltaTime));
        }

        bool CanSprint()
        {
            if (!_isSprinting) return false;
            if (_staminaDepleted) return false;
            if (_currentStamina.Value <= 0) return false;

            return true;
        }

        void DepleteStamina()
        {
            if (!FightManager.IsFightActive) return;
            if (!_isSprinting || _staminaDepleted) return;

            _currentStamina.ApplyChange(-_staminaDepletionRate * Time.deltaTime);
            if (_currentStamina.Value <= 0)
            {
                _staminaDepleted = true;
                _isSprinting = false;
                _currentStamina.Value = 0;
            }
        }

        void RefillStamina()
        {
            if (!CanRefillStamina()) return;

            if (_currentStamina.Value < _maxStamina.GetValue())
                _currentStamina.ApplyChange(_staminaRefillRate * Time.deltaTime);
        }

        bool CanRefillStamina()
        {
            if (!FightManager.IsFightActive) return true;

            return !_isSprinting;
        }

        void SetSpeed(float targetSpeed)
        {
            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon
            Vector3 velocity = _controller.velocity;
            float currentHorizontalSpeed = new Vector3(velocity.x, 0.0f,
                velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _inputDirection.magnitude;
            _speed = targetSpeed;
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f; // round speed to 3 decimal places
            }
        }

        float GetGravity()
        {
            _groundedPlayer = IsGrounded();
            if (_groundedPlayer && _playerVelocity.y < 0)
                _playerVelocity.y = 0f;
            if (!_groundedPlayer)
                _playerVelocity.y += _gravityValue * Time.deltaTime;
            return _playerVelocity.y;
        }

        bool IsGrounded()
        {
            return transform.position.y < 0.02f;
        }

        void AssignAnimationIDs()
        {
            _animVelocityX = Animator.StringToHash("VelocityX");
            _animVelocityZ = Animator.StringToHash("VelocityZ");
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

            _inputDirection = new(inputValue.x, 0, inputValue.y);
        }

        void ResetMovementVector(InputAction.CallbackContext context)
        {
            _inputDirection = Vector3.zero;

            // recenter transform, is it a good idea? It's here just in case... xD
            if (_animator == null) return;
            _animator.transform.DOKill();
            _animator.transform.DOLocalMove(Vector3.zero, 1f);
        }

        public void SetMoveSpeed(float speed)
        {
            MoveSpeed = speed;
        }


        void SetAnimationBlend()
        {
            _animationBlend = Mathf.Lerp(_animationBlend, _speed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = _inputDirection.normalized;
            Transform t = _gfx.transform;
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
            _gfx.rotation = Quaternion.Euler(new(0, lookRotation.eulerAngles.y, 0));
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
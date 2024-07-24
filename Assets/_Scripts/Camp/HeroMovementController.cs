using Cinemachine;
using DG.Tweening;
using Lis.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis
{
    public class HeroMovementController : MonoBehaviour
    {
        [Header("Player Movement")] [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 7.0f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        readonly float _zoomMax = 60f;
        readonly float _zoomMin = 20f;

        // camera zoom
        readonly float _zoomSpeed = 0.05f;
        float _animationBlend;

        Animator _animator;

        // animation IDs
        int _animVelocityX;
        int _animVelocityZ;

        Camera _cam;
        CinemachineVirtualCamera _cinemachineVirtualCamera;
        CharacterController _controller;

        GameManager _gameManager;
        Transform _gfx;
        Vector3 _inputDirection;

        // player
        Mouse _mouse;
        PlayerInput _playerInput;

        Vector3 _playerVelocity;
        float _rotationVelocity;
        float _speed;
        float _targetRotation;
        float _targetZoom;


        void Start()
        {
            _cam = Camera.main;
            _mouse = Mouse.current;
            _gameManager = GameManager.Instance;

            _cinemachineVirtualCamera = CampManager.Instance.HeroFollowCamera;
            _targetZoom = _cinemachineVirtualCamera.m_Lens.FieldOfView;

            _animator = GetComponentInChildren<Animator>();
            _controller = GetComponent<CharacterController>();
            _gfx = _animator.transform;

            AssignAnimationIDs();
        }

        void Update()
        {
            SetAnimationBlend();
            Move();
        }

        void LateUpdate()
        {
            RotateTowardsMouse();
            ZoomCameraSmoothly();
        }

        /* INPUT */
        void OnEnable()
        {
            if (_gameManager == null)
                _gameManager = GameManager.Instance;

            _playerInput = _gameManager.GetComponent<PlayerInput>();
            _playerInput.SwitchCurrentActionMap("Arena");
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


        void Move()
        {
            float targetSpeed = MoveSpeed;
            SetSpeed(targetSpeed);

            Vector3 moveDir = _inputDirection.normalized;

            _controller.Move(moveDir * (_speed * Time.deltaTime));
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

        void AssignAnimationIDs()
        {
            _animVelocityX = Animator.StringToHash("VelocityX");
            _animVelocityZ = Animator.StringToHash("VelocityZ");
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
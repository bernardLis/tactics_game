using System;
using Cinemachine;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis.Map
{
    public class CameraController : Singleton<CameraController>
    {
        GameManager _gameManager;
        PlayerInput _playerInput;

        CinemachineVirtualCamera _cinemachineVirtualCamera;

        PlayerController _playerController;
        [SerializeField] CameraFollowController _followTransform;

        readonly float _zoomDefault = 4f;
        readonly float _zoomMax = 10f;
        readonly float _zoomMin = 2f;

        readonly float _zoomSpeed = 0.01f;

        float _targetZoom;

        Vector2 _inputDirection;
        readonly float _arrowMoveSpeed = 5f;

        void Start()
        {
            _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
            _targetZoom = _zoomDefault;

            _playerController = PlayerController.Instance;
        }

        /* INPUT */
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
            _playerInput.actions["ZoomCamera"].performed += ZoomCamera;
            _playerInput.actions["Space"].performed += DefaultCamera;
            _playerInput.actions["PlayerMovement"].performed += ArrowMovement;
            _playerInput.actions["PlayerMovement"].canceled += ResetMovementVector;
        }


        void UnsubscribeInputActions()
        {
            _playerInput.actions["ZoomCamera"].performed -= ZoomCamera;
            _playerInput.actions["Space"].performed -= DefaultCamera;
            _playerInput.actions["PlayerMovement"].performed -= ArrowMovement;
            _playerInput.actions["PlayerMovement"].canceled -= ResetMovementVector;
        }


        void LateUpdate()
        {
            Move();
            ZoomCameraSmoothly();
        }

        /* MOVE */
        void ArrowMovement(InputAction.CallbackContext context)
        {
            _followTransform.StopFollowingPlayer();

            Vector3 inputValue = context.ReadValue<Vector2>();
            _inputDirection = inputValue.normalized;
        }

        void ResetMovementVector(InputAction.CallbackContext context)
        {
            _inputDirection = Vector3.zero;
        }

        void Move()
        {
            if (_inputDirection == Vector2.zero) return;
            _cinemachineVirtualCamera.Follow = _followTransform.transform;

            _followTransform.transform.position +=
                new Vector3(_inputDirection.x, _inputDirection.y, 0) * (Time.deltaTime * _arrowMoveSpeed);
        }

        /* ZOOM */
        void ZoomCamera(InputAction.CallbackContext ctx)
        {
            // so it is 0,120 and 0,-120 on mouse scroll
            Vector2 scrollValue = ctx.ReadValue<Vector2>();

            float newValue = _cinemachineVirtualCamera.m_Lens.OrthographicSize - scrollValue.y * _zoomSpeed;
            _targetZoom = Mathf.Clamp(newValue, _zoomMin, _zoomMax);
        }

        void ZoomCameraSmoothly()
        {
            float newValue = Mathf.Lerp(_cinemachineVirtualCamera.m_Lens.OrthographicSize, _targetZoom,
                Time.deltaTime * 5);
            _cinemachineVirtualCamera.m_Lens.OrthographicSize = newValue;
        }

        void DefaultCamera(InputAction.CallbackContext obj)
        {
            DefaultCamera();
        }

        public void DefaultCamera()
        {
            _followTransform.FollowPlayer();
            _targetZoom = _zoomDefault;
            _cinemachineVirtualCamera.Follow = _playerController.transform;
        }
    }
}
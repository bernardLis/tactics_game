using System;
using System.Collections;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis.Camp
{
    public class GrabManager : Singleton<GrabManager>
    {
        AudioManager _audioManager;

        Camera _cam;
        CursorManager _cursorManager;
        GameManager _gameManager;

        GameObject _grabbedObject;
        bool _isGrabbingEnabled;

        Mouse _mouse;
        float _objectYPosition;
        PlayerInput _playerInput;

        bool _pointerDown;

        bool _wasInitialized;

        public event Action OnGrabbed;
        public event Action OnReleased;

        public void Initialize()
        {
            if (_wasInitialized) return;
            _wasInitialized = true;

            _gameManager = GameManager.Instance;
            _audioManager = AudioManager.Instance;
            _cursorManager = CursorManager.Instance;
            _playerInput = _gameManager.GetComponent<PlayerInput>();

            _cam = Camera.main;
            _mouse = Mouse.current;

            EnableGrabbing();
        }

        void EnableGrabbing()
        {
            if (this == null) return;
            _isGrabbingEnabled = true;
        }

        public void TryGrabbing(GameObject obj, float yPosition = 0f)
        {
            if (!IsGrabbingAllowed()) return;

            _pointerDown = true;
            _cursorManager.SetCursorByName("Grab");

            StartCoroutine(GrabCoroutine(obj, yPosition));
            OnGrabbed?.Invoke();
        }

        IEnumerator GrabCoroutine(GameObject obj, float yPosition = 0f)
        {
            yield return new WaitForSeconds(0.5f);
            if (!_pointerDown) yield break;

            _objectYPosition = obj.transform.position.y;
            if (yPosition != 0f) _objectYPosition = yPosition;

            _audioManager.CreateSound()
                .WithSound(_audioManager.GetSound("Grab"))
                .WithPosition(obj.transform.position)
                .Play();

            _cursorManager.SetCursorByName("Hold");

            _grabbedObject = obj;
            if (_grabbedObject.TryGetComponent(out IGrabbable g))
                g.Grabbed();

            StartCoroutine(UpdateGrabbedObjectPosition());
        }

        IEnumerator UpdateGrabbedObjectPosition()
        {
            while (_grabbedObject != null)
            {
                Vector3 mousePosition = _mouse.position.ReadValue();
                Ray ray = _cam.ScreenPointToRay(mousePosition);
                if (!Physics.Raycast(ray, out RaycastHit hit, 100, 1 << LayerMask.NameToLayer("Floor")))
                    yield return new WaitForSeconds(0.2f);

                Vector3 pos = new(hit.point.x, _objectYPosition, hit.point.z);
                if (!IsPositionValid(pos)) yield break;
                _grabbedObject.transform.position = pos;

                yield return new WaitForFixedUpdate();
            }
        }

        bool IsPositionValid(Vector3 pos)
        {
            return true;
        }

        bool IsGrabbingAllowed()
        {
            if (!_wasInitialized) return false;
            if (_grabbedObject != null) return false;
            if (!_isGrabbingEnabled) return false;

            return true;
        }

        void OnPointerUp(InputAction.CallbackContext context)
        {
            if (!_wasInitialized) return;
            if (this == null) return;

            _pointerDown = false;
            _cursorManager.ClearCursor();

            if (_grabbedObject == null) return;

            _audioManager.CreateSound()
                .WithSound(_audioManager.GetSound("Grab"))
                .WithPosition(_grabbedObject.transform.position)
                .Play();
            if (_grabbedObject.TryGetComponent(out IGrabbable g))
                g.Released();
            _grabbedObject = null;

            OnReleased?.Invoke();
            StopAllCoroutines();
        }

        public void CancelGrabbing()
        {
            OnPointerUp(default);
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

        void SubscribeInputActions()
        {
            _playerInput.actions["LeftMouseClick"].canceled += OnPointerUp;
        }

        void UnsubscribeInputActions()
        {
            _playerInput.actions["LeftMouseClick"].canceled -= OnPointerUp;
        }
    }
}
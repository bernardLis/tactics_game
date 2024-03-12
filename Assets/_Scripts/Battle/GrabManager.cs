using System.Collections;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis.Battle
{
    public class GrabManager : Singleton<GrabManager>
    {
        GameManager _gameManager;
        AudioManager _audioManager;
        CursorManager _cursorManager;
        PlayerInput _playerInput;

        Camera _cam;
        Mouse _mouse;

        bool _isGrabbingUnlocked;
        bool _isGrabbingEnabled;

        bool _pointerDown;

        GameObject _grabbedObject;
        float _objectYPosition;

        bool _wasInitialized;

        public void Initialize()
        {
            if (_wasInitialized) return;
            _wasInitialized = true;

            _gameManager = GameManager.Instance;
            _audioManager = AudioManager.Instance;
            _cursorManager = CursorManager.Instance;
            _playerInput = _gameManager.GetComponent<PlayerInput>();

            _isGrabbingUnlocked = _gameManager.UpgradeBoard.GetUpgradeByName("Hero Grab").CurrentLevel != -1;
            EnableGrabbing();
        }

        public void EnableGrabbing()
        {
            if (BattleManager.BlockBattleInput) return;
            if (this == null) return;
            if (!_isGrabbingUnlocked) return;

            _isGrabbingEnabled = true;
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
            _playerInput.actions["LeftMouseClick"].canceled += OnPointerUp;
        }

        void UnsubscribeInputActions()
        {
            _playerInput.actions["LeftMouseClick"].canceled -= OnPointerUp;
        }

        public void TryGrabbing(GameObject obj, float yPosition = 0f)
        {
            if (!IsGrabbingAllowed()) return;

            _pointerDown = true;
            _cursorManager.SetCursorByName("Grab");

            StartCoroutine(GrabCoroutine(obj, yPosition));
        }

        IEnumerator GrabCoroutine(GameObject obj, float yPosition = 0f)
        {
            yield return new WaitForSeconds(0.5f);
            if (!_pointerDown) yield break;

            _objectYPosition = obj.transform.position.y;
            if (yPosition != 0f) _objectYPosition = yPosition;

            _audioManager.PlaySfx("Grab", obj.transform.position);
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
                _grabbedObject.transform.position = pos;

                yield return new WaitForFixedUpdate();
            }
        }

        public bool IsGrabbingAllowed()
        {
            if (!_wasInitialized) return false;
            if (_grabbedObject != null) return false;
            if (!_isGrabbingEnabled) return false;

            return true;
        }

        public void OnPointerUp(InputAction.CallbackContext context)
        {
            if (!_wasInitialized) return;
            if (this == null) return;

            _pointerDown = false;
            _cursorManager.ClearCursor();

            if (_grabbedObject == null) return;

            _audioManager.PlaySfx("Grab", _grabbedObject.transform.position);

            if (_grabbedObject.TryGetComponent(out IGrabbable g))
                g.Released();
            _grabbedObject = null;

            StopAllCoroutines();
        }

        public void CancelGrabbing()
        {
            OnPointerUp(default);
        }
    }
}
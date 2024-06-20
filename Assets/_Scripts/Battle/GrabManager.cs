using System.Collections;
using Lis.Battle.Arena;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis.Battle
{
    public class GrabManager : Singleton<GrabManager>
    {
        ArenaManager _arenaManager;
        AudioManager _audioManager;

        Camera _cam;
        CursorManager _cursorManager;
        GameManager _gameManager;

        GameObject _grabbedObject;
        bool _isGrabbingEnabled;

        bool _isGrabbingUnlocked;
        Mouse _mouse;
        float _objectYPosition;
        PlayerInput _playerInput;

        bool _pointerDown;

        bool _wasInitialized;

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

        public void Initialize()
        {
            if (_wasInitialized) return;
            _wasInitialized = true;

            _gameManager = GameManager.Instance;
            _audioManager = AudioManager.Instance;
            _cursorManager = CursorManager.Instance;
            _playerInput = _gameManager.GetComponent<PlayerInput>();

            _arenaManager = GetComponent<ArenaManager>();

            _cam = Camera.main;
            _mouse = Mouse.current;

            _isGrabbingUnlocked = true; // _gameManager.UpgradeBoard.GetUpgradeByName("Hero Grab").CurrentLevel != -1;
            EnableGrabbing();
        }

        void EnableGrabbing()
        {
            if (BattleManager.BlockBattleInput) return;
            if (this == null) return;
            if (!_isGrabbingUnlocked) return;

            _isGrabbingEnabled = true;
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
            if (!FightManager.IsFightActive)
            {
                if (_arenaManager.IsPositionInPlayerLockerRoom(pos)) return true;
                CancelGrabbing();
                return false;
            }

            if (!_arenaManager.IsPositionInArena(pos))
            {
                CancelGrabbing();
                return false;
            }

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

            StopAllCoroutines();
        }

        public void CancelGrabbing()
        {
            OnPointerUp(default);
        }
    }
}
using System;
using Lis.Battle.Fight;
using Lis.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis.Battle
{
    public class InputManager : MonoBehaviour
    {
        GameManager _gameManager;

        MenuScreen _menuScreen;
        PlayerInput _playerInput;

        void Start()
        {
            _gameManager = GameManager.Instance;
            _playerInput = _gameManager.GetComponent<PlayerInput>();
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

        public event Action OnLeftMouseClick;
        public event Action OnRightMouseClick;

        public event Action OnOneClicked;
        public event Action OnTwoClicked;
        public event Action OnThreeClicked;
        public event Action OnFourClicked;

        public event Action OnSpaceClicked;
        public event Action OnEnterClicked;

        void SubscribeInputActions()
        {
            _playerInput.actions["ToggleMenu"].performed += OpenMenu;
#if UNITY_EDITOR
            _playerInput.actions["LeftMouseClick"].performed += LeftMouseClicked;
            _playerInput.actions["RightMouseClick"].performed += RightMouseClicked;

            _playerInput.actions["Space"].performed += SpaceClicked;
            _playerInput.actions["Enter"].performed += EnterClicked;

            _playerInput.actions["1"].performed += OneClicked;
            _playerInput.actions["2"].performed += TwoClicked;
            _playerInput.actions["3"].performed += ThreeClicked;
            _playerInput.actions["4"].performed += FourClicked;

            _playerInput.actions["DebugSpawnMinionWave"].performed += DebugSpawnMinionWave;
            _playerInput.actions["DebugSpawnTile"].performed += DebugSpawnTile;
            _playerInput.actions["DebugSpawnBoss"].performed += DebugSpawnBoss;
            _playerInput.actions["DebugKillHero"].performed += DebugKillHero;
#endif
        }

        void UnsubscribeInputActions()
        {
            _playerInput.actions["ToggleMenu"].performed -= OpenMenu;
#if UNITY_EDITOR

            _playerInput.actions["LeftMouseClick"].performed -= LeftMouseClicked;
            _playerInput.actions["RightMouseClick"].performed -= RightMouseClicked;

            _playerInput.actions["Space"].performed -= SpaceClicked;
            _playerInput.actions["Enter"].performed -= EnterClicked;

            _playerInput.actions["1"].performed -= OneClicked;
            _playerInput.actions["2"].performed -= TwoClicked;
            _playerInput.actions["3"].performed -= ThreeClicked;
            _playerInput.actions["4"].performed -= FourClicked;

            _playerInput.actions["DebugSpawnMinionWave"].performed -= DebugSpawnMinionWave;
            _playerInput.actions["DebugSpawnTile"].performed -= DebugSpawnTile;
            _playerInput.actions["DebugSpawnBoss"].performed -= DebugSpawnBoss;
            _playerInput.actions["DebugKillHero"].performed -= DebugKillHero;
#endif
        }

        /* DEBUG inputs */
        void LeftMouseClicked(InputAction.CallbackContext ctx)
        {
            OnLeftMouseClick?.Invoke();
        }

        void RightMouseClicked(InputAction.CallbackContext ctx)
        {
            OnRightMouseClick?.Invoke();
        }

        void SpaceClicked(InputAction.CallbackContext ctx)
        {
            OnSpaceClicked?.Invoke();
        }

        void EnterClicked(InputAction.CallbackContext ctx)
        {
            OnEnterClicked?.Invoke();
        }

        void OneClicked(InputAction.CallbackContext ctx)
        {
            OnOneClicked?.Invoke();
        }

        void TwoClicked(InputAction.CallbackContext ctx)
        {
            OnTwoClicked?.Invoke();
        }

        void ThreeClicked(InputAction.CallbackContext ctx)
        {
            OnThreeClicked?.Invoke();
        }

        void FourClicked(InputAction.CallbackContext ctx)
        {
            OnFourClicked?.Invoke();
        }

        void DebugSpawnMinionWave(InputAction.CallbackContext ctx)
        {
        }

        void DebugSpawnTile(InputAction.CallbackContext ctx)
        {
        }

        void DebugSpawnBoss(InputAction.CallbackContext ctx)
        {
            BattleManager.Instance.GetComponent<BossManager>().SpawnBoss();
        }

        void DebugKillHero(InputAction.CallbackContext ctx)
        {
        }

        public void OpenMenu(InputAction.CallbackContext ctx)
        {
            if (_gameManager.OpenFullScreens.Count > 0) return;
            if (_menuScreen != null) return;

            _menuScreen = new();
            _menuScreen.OnHide += MenuScreenClosed;
        }

        void MenuScreenClosed()
        {
            _menuScreen = null;
        }
    }
}
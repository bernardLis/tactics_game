using System;


using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis
{
    public class BattleInputManager : MonoBehaviour
    {
        GameManager _gameManager;
        PlayerInput _playerInput;

        MenuScreen _menuScreen;

        public event Action OnLeftMouseClick;
        public event Action OnRightMouseClick;

        public event Action OnSpaceClicked;
        public event Action OnEnterClicked;
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

        void SubscribeInputActions()
        {
            _playerInput.actions["ToggleMenu"].performed += OpenMenu;

            _playerInput.actions["LeftMouseClick"].performed += LeftMouseClicked;
            _playerInput.actions["RightMouseClick"].performed += RightMouseClicked;

            _playerInput.actions["Space"].performed += SpaceClicked;
            _playerInput.actions["Enter"].performed += EnterClicked;
        
            _playerInput.actions["DebugSpawnMinionWave"].performed += DebugSpawnMinionWave;
            _playerInput.actions["DebugSpawnTile"].performed += DebugSpawnTile;
            _playerInput.actions["DebugSpawnBossTile"].performed += DebugSpawnBossTile;
            _playerInput.actions["DebugKillHero"].performed += DebugKillHero;
        }

        void UnsubscribeInputActions()
        {
            _playerInput.actions["ToggleMenu"].performed -= OpenMenu;

            _playerInput.actions["LeftMouseClick"].performed -= LeftMouseClicked;
            _playerInput.actions["RightMouseClick"].performed -= RightMouseClicked;

            _playerInput.actions["Space"].performed -= SpaceClicked;
            _playerInput.actions["Enter"].performed -= EnterClicked;

            _playerInput.actions["DebugSpawnMinionWave"].performed -= DebugSpawnMinionWave;
            _playerInput.actions["DebugSpawnTile"].performed -= DebugSpawnTile;
            _playerInput.actions["DebugSpawnBossTile"].performed -= DebugSpawnBossTile;
            _playerInput.actions["DebugKillHero"].performed -= DebugKillHero;
        }

        /* DEBUG inputs */
        void LeftMouseClicked(InputAction.CallbackContext ctx) => OnLeftMouseClick?.Invoke();
        void RightMouseClicked(InputAction.CallbackContext ctx) => OnRightMouseClick?.Invoke();
        void SpaceClicked(InputAction.CallbackContext ctx) => OnSpaceClicked?.Invoke();
        void EnterClicked(InputAction.CallbackContext ctx) => OnEnterClicked?.Invoke();

        void DebugSpawnMinionWave(InputAction.CallbackContext ctx)
        {
            BattleManager.Instance.GetComponent<BattleMinionManager>().SpawnWave();
        }

        void DebugSpawnTile(InputAction.CallbackContext ctx)
        {
            BattleManager.Instance.GetComponent<BattleAreaManager>().UnlockNextTile();
        }

        void DebugSpawnBossTile(InputAction.CallbackContext ctx)
        {
            BattleManager.Instance.GetComponent<BattleAreaManager>().DebugSpawnBossTile();
        }

        void DebugKillHero(InputAction.CallbackContext ctx)
        {
            BattleManager.Instance.BattleHero.BaseGetHit(1000, default);
        }

        public void OpenMenu(InputAction.CallbackContext ctx)
        {
            if (_gameManager.OpenFullScreens.Count > 0) return;
            if (_menuScreen != null) return;

            _menuScreen = new MenuScreen();
            _menuScreen.OnHide += MenuScreenClosed;
        }

        void MenuScreenClosed()
        {
            _menuScreen = null;
        }

    }
}

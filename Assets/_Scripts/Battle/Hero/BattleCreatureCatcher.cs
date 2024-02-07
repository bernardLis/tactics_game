using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis
{
    public class BattleCreatureCatcher : PoolManager<BattleFriendBall>
    {
        [SerializeField] GameObject _friendsBallPrefab;

        GameManager _gameManager;
        PlayerInput _playerInput;

        public void Initialize()
        {
            CreatePool(_friendsBallPrefab, 20);
        }

        void ThrowBall(InputAction.CallbackContext context)
        {
            if (this == null) return;
            if (!context.performed) return;

            BattleFriendBall ball = GetObjectFromPool();
            ball.transform.position = transform.position;
            ball.gameObject.SetActive(true);
            ball.Throw();
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
            _playerInput.actions["ThrowBall"].performed += ThrowBall;
        }

        void UnsubscribeInputActions()
        {
            _playerInput.actions["ThrowBall"].performed -= ThrowBall;
        }
    }
}
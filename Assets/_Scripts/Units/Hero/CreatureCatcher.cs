using System;
using System.Collections;
using Lis.Battle;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Creature;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero
{
    public class CreatureCatcher : PoolManager<FriendBallController>
    {
        GameManager _gameManager;
        AudioManager _audioManager;
        PlayerInput _playerInput;

        HeroController _heroController;

        [SerializeField] GameObject _throwIndicatorPrefab;
        FriendBallThrowIndicator _throwIndicator;

        [SerializeField] GameObject _friendsBallPrefab;

        [SerializeField] Sound _throwSound;
        [SerializeField] Sound _perfectThrow;
        [SerializeField] Sound _noMoreBallsSound;


        // ok, so until 2 seconds is weak, 2-3 perfect, 3-5 too strong
        // TODO: magic numbers
        const float _maxChargeTime = 5;
        const float _maxThrowDistance = 20;
        float _throwCharge;
        IEnumerator _throwChargeCoroutine;
        bool _thrown;

        public event Action OnBallThrown;

        public void Initialize(HeroController heroController)
        {
            _audioManager = AudioManager.Instance;

            _throwIndicator = Instantiate(_throwIndicatorPrefab, BattleManager.Instance.EntityHolder)
                .GetComponent<FriendBallThrowIndicator>();
            _throwIndicator.gameObject.SetActive(false);

            CreatePool(_friendsBallPrefab, 20);
            _heroController = heroController;
        }

        void StartBallThrow(InputAction.CallbackContext context)
        {
            if (this == null) return;
            if (!_heroController.Hero.HasFriendBalls())
            {
                _audioManager.PlaySFX(_noMoreBallsSound, transform.position);
                _heroController.DisplayFloatingText("No friend balls.", Color.red);
                return;
            }

            if (_throwChargeCoroutine != null)
                StopCoroutine(_throwChargeCoroutine);

            _thrown = false;
            _throwChargeCoroutine = ThrowChargeCoroutine();
            StartCoroutine(_throwChargeCoroutine);
        }

        IEnumerator ThrowChargeCoroutine()
        {
            // HERE: charge throw sound
            _throwIndicator.gameObject.SetActive(true);
            _throwIndicator.Show();

            _throwCharge = 0;
            while (_throwCharge < _maxChargeTime)
            {
                _throwCharge += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            EndBallThrow(default);
        }

        void EndBallThrow(InputAction.CallbackContext context)
        {
            if (this == null) return;
            if (_throwChargeCoroutine != null)
                StopCoroutine(_throwChargeCoroutine);

            // if I force throw ball, I don't want to throw it again when I release button
            if (_thrown) return;
            _thrown = true;

            OnBallThrown?.Invoke();
            _heroController.Hero.UseFriendBall();
            _audioManager.PlaySFX(_throwSound, transform.position);
            _throwIndicator.EndShow();

            if (HandlePerfectThrow()) return;

            Vector3 throwPosition = CalculateThrowPosition();
            FriendBallController ballController = InitializeBall();
            ballController.Throw(transform.rotation, throwPosition);
        }

        bool HandlePerfectThrow()
        {
            if (_throwCharge is < 2 or > 3) return false;
            CreatureController bc = _throwIndicator.GetCreature();
            if (bc == null) return false;

            _audioManager.PlaySFX(_perfectThrow, transform.position);
            _heroController.DisplayFloatingText("Perfect throw!", Color.green);
            FriendBallController ballController = InitializeBall();
            ballController.PerfectThrow(transform.rotation, bc);
            return true;
        }

        FriendBallController InitializeBall()
        {
            FriendBallController ballController = GetObjectFromPool();
            ballController.transform.position = transform.position;
            ballController.gameObject.SetActive(true);
            return ballController;
        }

        Vector3 CalculateThrowPosition()
        {
            Vector3 heroPos = transform.position;
            Vector3 indicatorPos = _throwIndicator.transform.position;
            if (_throwCharge is >= 2 and <= 3)
                return indicatorPos;

            // if throw charge less then 2 throw closer than indicator position
            if (_throwCharge < 2)
            {
                _heroController.DisplayFloatingText("Weak throw!", Color.yellow);

                Vector3 direction = indicatorPos - heroPos;
                float distance = direction.magnitude;
                float throwDistance = Random.Range(0, distance);
                return heroPos + direction.normalized * throwDistance;
            }

            // if throw charge more than 3 throw further than indicator position
            if (_throwCharge > 3)
            {
                _heroController.DisplayFloatingText("Overthrown!", Color.red);

                Vector3 brokenPos = indicatorPos + Vector3.right * Random.Range(-1, 1) * (_throwCharge - 2);
                Vector3 direction = brokenPos - heroPos;
                float distance = direction.magnitude;
                float throwDistance = Random.Range(distance, _maxThrowDistance);
                return heroPos + direction.normalized * throwDistance;
            }

            return Vector3.zero;
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
            _playerInput.actions["ThrowBall"].started += StartBallThrow;
            _playerInput.actions["ThrowBall"].canceled += EndBallThrow;
        }

        void UnsubscribeInputActions()
        {
            _playerInput.actions["ThrowBall"].started -= StartBallThrow;
            _playerInput.actions["ThrowBall"].canceled += EndBallThrow;
        }
    }
}
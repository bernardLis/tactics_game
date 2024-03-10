using System.Collections;
using DG.Tweening;
using Lis.Battle;
using Lis.Core;
using Lis.Units;
using Lis.Units.Creature;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis
{
    public class CreatureRecaller : MonoBehaviour
    {
        GameManager _gameManager;
        PlayerInput _playerInput;
        BattleManager _battleManager;
        MovementController _heroMovementController;

        [SerializeField] GameObject _recallEffect;

        IEnumerator _recallCoroutine;

        public void Initialize(HeroController heroController)
        {
            // TODO: creature teleporter sounds
            _battleManager = BattleManager.Instance;
            _heroMovementController = heroController.GetComponent<MovementController>();
        }

        void StartCreatureRecall(InputAction.CallbackContext context)
        {
            if (this == null) return;
            Debug.Log("Creature recall started");
            if (_recallCoroutine != null) return;
            _recallCoroutine = RecallCoroutine();
            StartCoroutine(_recallCoroutine);
        }


        IEnumerator RecallCoroutine()
        {
            _heroMovementController.DisableMovement();

            // there is a visual effect
            _recallEffect.transform.localScale = Vector3.one;
            _recallEffect.SetActive(true);

            int totalDelay = 3;
            int currentDelay = 0;
            while (currentDelay < totalDelay)
            {
                yield return new WaitForSeconds(1);
                currentDelay++;
                Debug.Log("Recalling creatures: " + currentDelay + "s");
            }

            float creatureRecallDelay = 1.5f / _battleManager.Hero.Troops.Count;
            foreach (UnitController unit in _battleManager.PlayerEntities)
            {
                if (unit.Unit is not Creature) continue;
                if (Vector3.Distance(transform.position, unit.transform.position) < 5) continue;
                unit.RecallToHero();
                yield return new WaitForSeconds(creatureRecallDelay);
            }

            EndCreatureRecall(default);
        }

        void EndCreatureRecall(InputAction.CallbackContext context)
        {
            if (this == null) return;
            if (_recallCoroutine != null) StopCoroutine(_recallCoroutine);
            _heroMovementController.EnableMovement();
            _recallEffect.transform.DOScale(0, 1f).OnComplete(() =>
            {
                _recallEffect.SetActive(false);
                _recallCoroutine = null;
            });
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
            _playerInput.actions["RecallCreatures"].started += StartCreatureRecall;
            _playerInput.actions["RecallCreatures"].canceled += EndCreatureRecall;
        }

        void UnsubscribeInputActions()
        {
            _playerInput.actions["RecallCreatures"].started -= StartCreatureRecall;
            _playerInput.actions["RecallCreatures"].canceled += EndCreatureRecall;
        }
    }
}
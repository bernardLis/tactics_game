using System.Collections.Generic;
using Lis.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis.Units.Hero
{
    public class Interactor : MonoBehaviour
    {
        private readonly List<IInteractable> _interactables = new();
        private GameManager _gameManager;
        private PlayerInput _playerInput;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _playerInput = _gameManager.GetComponent<PlayerInput>();
        }

        /* INPUT */
        private void OnEnable()
        {
            if (_gameManager == null)
                _gameManager = GameManager.Instance;

            _playerInput = _gameManager.GetComponent<PlayerInput>();
            _playerInput.SwitchCurrentActionMap("Battle");
            UnsubscribeInputActions();
            SubscribeInputActions();
        }

        private void OnDisable()
        {
            if (_playerInput == null) return;

            UnsubscribeInputActions();
        }

        private void OnDestroy()
        {
            if (_playerInput == null) return;

            UnsubscribeInputActions();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable interactable)) return;
            _interactables.Add(interactable);
            interactable.DisplayTooltip();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable interactable)) return;
            _interactables.Remove(interactable);
            interactable.HideTooltip();
        }

        private void Interact(InputAction.CallbackContext context)
        {
            // TODO: get closest interactable not first
            if (_interactables.Count == 0) return;
            foreach (IInteractable interactable in _interactables)
            {
                if (!interactable.CanInteract()) continue;
                if (!interactable.Interact(this)) continue;
                _interactables.Remove(interactable);
                interactable.HideTooltip();
                break;
            }
        }

        private void SubscribeInputActions()
        {
            _playerInput.actions["Interact"].performed += Interact;
        }

        private void UnsubscribeInputActions()
        {
            _playerInput.actions["Interact"].performed -= Interact;
        }
    }
}
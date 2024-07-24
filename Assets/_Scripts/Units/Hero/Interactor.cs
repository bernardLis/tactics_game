using System.Collections.Generic;
using Lis.Arena;
using Lis.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis.Units.Hero
{
    public class Interactor : MonoBehaviour
    {
        readonly List<IInteractable> _interactables = new();
        GameManager _gameManager;
        PlayerInput _playerInput;
        TooltipManager _tooltipManager;

        void Start()
        {
            _gameManager = GameManager.Instance;
            _playerInput = _gameManager.GetComponent<PlayerInput>();
            _tooltipManager = TooltipManager.Instance;
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

        void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable interactable)) return;
            if (!interactable.CanInteract()) return;
            _interactables.Add(interactable);
            ShowInteractionPrompt(interactable);
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable interactable)) return;
            _interactables.Remove(interactable);
            HideInteractionPrompt();
        }

        void Interact(InputAction.CallbackContext context)
        {
            // TODO: get closest interactable not first
            if (_interactables.Count == 0) return;
            foreach (IInteractable interactable in _interactables)
            {
                if (!interactable.CanInteract()) continue;
                if (!interactable.Interact(this)) continue;
                _interactables.Remove(interactable);

                HideInteractionPrompt();
                break;
            }
        }

        void ShowInteractionPrompt(IInteractable interactable)
        {
            _tooltipManager.ShowInteractionPrompt(interactable.InteractionPrompt);
        }

        void HideInteractionPrompt()
        {
            _tooltipManager.HideInteractionPrompt();
        }

        void SubscribeInputActions()
        {
            _playerInput.actions["Interact"].performed += Interact;
        }

        void UnsubscribeInputActions()
        {
            _playerInput.actions["Interact"].performed -= Interact;
        }
    }
}
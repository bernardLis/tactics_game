using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleInteractor : MonoBehaviour
{
    GameManager _gameManager;
    PlayerInput _playerInput;

    [SerializeField] Transform _interactionPoint;
    [SerializeField] float _interactionRadius = 1f;
    [SerializeField] LayerMask _interactionLayerMask;

    [SerializeField] int _colliderCount;
    [SerializeField] Collider[] _interactables = new Collider[3];
    [SerializeField] IInteractable _currentInteractable;

    void Start()
    {
        _interactionLayerMask = LayerMask.GetMask("Interactable");
        _gameManager = GameManager.Instance;

    }

    void Update()
    {
        _colliderCount = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionRadius,
                                          _interactables, _interactionLayerMask);
        HandleTooltip();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionRadius);
    }

    void HandleTooltip()
    {
        if (_colliderCount == 0)
        {
            if (_currentInteractable == null) return;

            _currentInteractable.HideTooltip();
            _currentInteractable = null;
            return;
        }

        if (_interactables[0].TryGetComponent(out IInteractable interactable))
        {
            if (_currentInteractable == interactable) return;
            if (_currentInteractable != null) _currentInteractable.HideTooltip();
            _currentInteractable = interactable;
            interactable.DisplayTooltip();
        }
    }

    void Interact(InputAction.CallbackContext context)
    {
        if (_currentInteractable == null) return;
        if (!_currentInteractable.CanInteract(this)) return;
        _currentInteractable.Interact(this);
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
        _playerInput.actions["Interact"].canceled += Interact;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["Interact"].canceled -= Interact;
    }

}

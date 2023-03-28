using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class DraggableArmies : MonoBehaviour
{
    const string _ussDragDropContainer = "dashboard__army-drag-drop-container";

    const string _ussArmySlotElement = "army-slot__main";
    const string _ussArmyElement = "army__main";

    GameManager _gameManager;
    DashboardManager _dashboardManager;
    PlayerInput _playerInput;

    bool _isShiftDown;

    VisualElement _root;

    bool _isDragging;

    ArmySlotElement _originalSlot;
    ArmySlotElement _newSlot;
    VisualElement _dragDropContainer;
    ArmyElement _draggedArmy;

    List<ArmySlotElement> _armySlots = new();
    List<ArmyElement> _armyElements = new();
    void Start()
    {
        _gameManager = GameManager.Instance;
        _dashboardManager = DashboardManager.Instance;

        _root = _dashboardManager.Root;
        _dragDropContainer = new VisualElement();
        _dragDropContainer.AddToClassList(_ussDragDropContainer);
        _root.Add(_dragDropContainer);
    }

    /* INPUT */
    void OnEnable()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        _playerInput = _gameManager.GetComponent<PlayerInput>();
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
        _playerInput.actions["Shift"].performed += ShiftDown;
        _playerInput.actions["Shift"].canceled += ShiftUp;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["Shift"].performed -= ShiftDown;
        _playerInput.actions["Shift"].canceled -= ShiftUp;
    }

    void ShiftDown(InputAction.CallbackContext ctx) { _isShiftDown = true; }
    void ShiftUp(InputAction.CallbackContext ctx) { _isShiftDown = false; }

    public void Initialize()
    {
        _armySlots = new();
        _armyElements = new();

        List<VisualElement> slots = _root.Query(className: _ussArmySlotElement).ToList();
        foreach (VisualElement item in slots)
        {
            ArmySlotElement el = (ArmySlotElement)item;
            if (el.IsLocked) continue;
            if (_armySlots.Contains(el)) continue;
            el.OnArmyAdded += AddDraggableArmyElement;
            _armySlots.Add(el);
        }

        List<VisualElement> els = _root.Query(className: _ussArmyElement).ToList();
        foreach (VisualElement item in els)
        {
            ArmyElement el = (ArmyElement)item;
            if (el.IsLocked) continue;
            AddDraggableArmyElement(el);
        }

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void AddDraggableArmyElement(ArmyElement el)
    {
        if (_armyElements.Contains(el)) return;

        el.RegisterCallback<PointerDownEvent>(OnArmyPointerDown);
        _armyElements.Add(el);
    }

    public void Reset()
    {
        _armySlots = new();
        _armyElements = new();

        _root.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.UnregisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnArmyPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        ArmyElement armyElement = (ArmyElement)evt.currentTarget;

        ArmySlotElement armySlotElement = null;
        if (armyElement.parent is ArmySlotElement)
        {
            armySlotElement = (ArmySlotElement)armyElement.parent;
            armySlotElement.RemoveArmy();
        }
        
        // TODO: if shift down, split 
        Debug.Log($"click click shift down: {_isShiftDown}");

        StartArmyDrag(evt.position, armySlotElement, armyElement);
    }

    void StartArmyDrag(Vector2 position, ArmySlotElement originalSlot, ArmyElement draggedItem)
    {
        _draggedArmy = draggedItem;
        _draggedArmy.style.position = Position.Absolute;
        _draggedArmy.style.top = 0;
        _draggedArmy.style.left = 0;

        //Set tracking variables
        _isDragging = true;
        _originalSlot = originalSlot;
        //Set the new position
        _dragDropContainer.BringToFront();
        _dragDropContainer.style.top = position.y - _dragDropContainer.layout.height / 2;
        _dragDropContainer.style.left = position.x - _dragDropContainer.layout.width / 2;
        //Set the image
        _dragDropContainer.Add(_draggedArmy);
        //Flip the visibility on
        _dragDropContainer.style.visibility = Visibility.Visible;
    }

    void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!_isDragging)
            return;

        if (_draggedArmy != null)
        {
            //Set the new position
            _dragDropContainer.style.left = evt.position.x - _dragDropContainer.layout.width / 2;
            _dragDropContainer.style.top = evt.position.y - _dragDropContainer.layout.height / 2;
        }
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging)
            return;

        if (_draggedArmy != null)
            HandleArmyPointerUp();
    }

    void HandleArmyPointerUp()
    {
        IEnumerable<ArmySlotElement> slots = _armySlots.Where(x =>
               x.worldBound.Overlaps(_dragDropContainer.worldBound));

        List<VisualElement> characterCards = _root.Query(className: "character-card-mini__main").ToList();
        IEnumerable<VisualElement> overlappingCards = characterCards.Where(x =>
                    x.worldBound.Overlaps(_dragDropContainer.worldBound));

        if (slots.Count() != 0)
        {
            AddArmyToClosestSlot(slots);
            return;
        }

        _originalSlot.AddArmy(_draggedArmy);
        DragCleanUp();
    }

    void AddArmyToClosestSlot(IEnumerable<ArmySlotElement> slots)
    {
        _newSlot = slots.OrderBy(x => Vector2.Distance
           (x.worldBound.position, _dragDropContainer.worldBound.position)).First();

        if (_newSlot.ArmyElement != null &&
            _newSlot.ArmyElement.ArmyGroup.ArmyEntity != _draggedArmy.ArmyGroup.ArmyEntity)
        {
            _originalSlot.AddArmy(_newSlot.ArmyElement);
            _newSlot.RemoveArmy();
        }

        _newSlot.AddArmy(_draggedArmy);
        DragCleanUp();
    }

    void DragCleanUp()
    {
        //Clear dragging related visuals and data
        _isDragging = false;

        _originalSlot = null;
        _draggedArmy = null;

        _dragDropContainer.Clear();
        _dragDropContainer.style.visibility = Visibility.Hidden;
        _gameManager.SaveJsonData();
    }

}

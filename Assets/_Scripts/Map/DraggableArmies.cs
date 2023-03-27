using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DraggableArmies : MonoBehaviour
{
    // HERE: change
    const string _ussDragDropContainer = "dashboard__item-drag-drop-container";

    const string _ussArmySlotElement = "army-slot__main";
    const string _ussArmyElement = "army__main";

    GameManager _gameManager;
    DashboardManager _dashboardManager;

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

    public void Initialize()
    {
        Debug.Log($"initialize");
        _armySlots = new();
        _armyElements = new();

        List<VisualElement> slots = _root.Query(className: _ussArmySlotElement).ToList();
        Debug.Log($"slots.Count {slots.Count}");
        foreach (VisualElement item in slots)
            _armySlots.Add((ArmySlotElement)item);

        List<VisualElement> els = _root.Query(className: _ussArmyElement).ToList();
        Debug.Log($"els.Count {els.Count}");

        foreach (VisualElement item in els)
        {
            item.RegisterCallback<PointerDownEvent>(OnArmyPointerDown);
            _armyElements.Add((ArmyElement)item);
        }

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
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
        Debug.Log($"click click");
        if (evt.button != 0)
            return;

        ArmyElement armyElement = (ArmyElement)evt.currentTarget;

        ArmySlotElement armySlotElement = null;
        if (armyElement.parent is ArmySlotElement)
        {
            armySlotElement = (ArmySlotElement)armyElement.parent;
            armySlotElement.RemoveArmy();
        }

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

        if (_newSlot.ArmyElement != null)
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

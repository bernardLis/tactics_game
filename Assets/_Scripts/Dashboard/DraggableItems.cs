using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;

public class DraggableItems : MonoBehaviour
{
    GameManager _gameManager;

    VisualElement _root;
    VisualElement _itemContainer;

    // drag & drop
    // https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
    bool _isDragging;

    ItemSlot _originalSlot;
    ItemSlot _newSlot;
    VisualElement _dragDropContainer;
    ItemElement _draggedItem;

    List<ItemSlot> _allSlots = new();

    List<HeroCardStats> _allCards = new();

    const string _ussDragDropContainer = "dashboard__item-drag-drop-container";

    public void Initialize(VisualElement root, VisualElement itemContainer)
    {
        _gameManager = GameManager.Instance;

        _root = root;
        _itemContainer = itemContainer;

        _dragDropContainer = new VisualElement();
        _dragDropContainer.AddToClassList(_ussDragDropContainer);
        _root.Add(_dragDropContainer);

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    public void AddDraggableItem(ItemElement itemElement) { itemElement.RegisterCallback<PointerDownEvent>(OnItemPointerDown); }

    public void AddSlot(ItemSlot slot) { _allSlots.Add(slot); }
    public void RemoveSlot(ItemSlot slot) { _allSlots.Remove(slot); }

    void OnItemPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        ItemElement itemElement = (ItemElement)evt.currentTarget;

        if (itemElement.IsShop && itemElement.Item.Price > _gameManager.Gold)
        {
            Helpers.DisplayTextOnElement(_root, itemElement, "Insufficient funds", Color.red);
            return;
        }

        ItemSlot itemSlot = null;
        if (itemElement.parent is ItemSlot)
        {
            itemSlot = (ItemSlot)itemElement.parent;
            itemSlot.RemoveItem();
        }

        StartItemDrag(evt.position, itemSlot, itemElement);
    }

    void StartItemDrag(Vector2 position, ItemSlot originalSlot, ItemElement draggedItem)
    {
        _draggedItem = draggedItem;
        _draggedItem.style.position = Position.Absolute;
        _draggedItem.style.top = 0;
        _draggedItem.style.left = 0;

        //Set tracking variables
        _isDragging = true;
        _originalSlot = originalSlot;
        //Set the new position
        _dragDropContainer.BringToFront();
        _dragDropContainer.style.top = position.y - _dragDropContainer.layout.height / 2;
        _dragDropContainer.style.left = position.x - _dragDropContainer.layout.width / 2;
        //Set the image
        _dragDropContainer.Add(_draggedItem);
        //Flip the visibility on
        _dragDropContainer.style.visibility = Visibility.Visible;
    }

    void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!_isDragging)
            return;

        if (_draggedItem != null)
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

        if (_draggedItem != null)
            HandleItemPointerUp();
    }

    void HandleItemPointerUp()
    {
        IEnumerable<ItemSlot> slots = _allSlots.Where(x =>
               x.worldBound.Overlaps(_dragDropContainer.worldBound));

        List<VisualElement> heroCards = _root.Query(className: "hero-card-mini__main").ToList();
        IEnumerable<VisualElement> overlappingCards = heroCards.Where(x =>
                    x.worldBound.Overlaps(_dragDropContainer.worldBound));

        if (slots.Count() != 0)
        {
            AddItemToClosestSlot(slots);
            return;
        }

        // drag item from desk to hero to add it to hero
        if (overlappingCards.Count() != 0)
        {
            AddItemToHero(overlappingCards);
            return;
        }

        _originalSlot.AddItem(_draggedItem);
        DragCleanUp();
    }

    void AddItemToClosestSlot(IEnumerable<ItemSlot> slots)
    {
        _newSlot = slots.OrderBy(x => Vector2.Distance
           (x.worldBound.position, _dragDropContainer.worldBound.position)).First();

        // no item swap when buying
        if (_newSlot.ItemElement != null && _draggedItem.IsShop)
        {
            _originalSlot.AddItem(_draggedItem);
            DragCleanUp();
            return;
        }

        if (_newSlot.ItemElement != null)
        {
            _originalSlot.AddItem(_newSlot.ItemElement);
            _newSlot.RemoveItem();
        }

        _newSlot.AddItem(_draggedItem);

        if (_draggedItem.IsShop)
        {
            _gameManager.ChangeGoldValue(-_draggedItem.Item.Price);
            _draggedItem.ItemBought();
        }

        DragCleanUp();
    }

    void AddItemToHero(IEnumerable<VisualElement> overlappingCards)
    {
        VisualElement closesEl = overlappingCards.OrderBy(x => Vector2.Distance
                         (x.worldBound.position, _dragDropContainer.worldBound.position)).First();
        HeroCardMini closestCard = (HeroCardMini)closesEl;
        closestCard.Hero.AddItem(_draggedItem.Item);
        if (_draggedItem.IsShop)
        {
            _gameManager.ChangeGoldValue(-_draggedItem.Item.Price);
            _draggedItem.ItemBought();
        }
        DragCleanUp();

    }

    void DragCleanUp()
    {
        //Clear dragging related visuals and data
        _isDragging = false;

        _originalSlot = null;
        _draggedItem = null;

        _dragDropContainer.Clear();
        _dragDropContainer.style.visibility = Visibility.Hidden;
        _gameManager.SaveJsonData();
    }

    public void RemoveDragContainer()
    {
        if (_dragDropContainer != null)
        {
            _root.Remove(_dragDropContainer);
            _dragDropContainer = null;
        }
    }
}

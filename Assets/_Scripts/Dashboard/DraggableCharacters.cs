using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;

public class DraggableCharacters : MonoBehaviour
{
    VisualElement _root;
    VisualElement _cardContainer;

    // drag & drop
    // https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
    bool _isDragging;

    CharacterCardMiniSlot _originalSlot;
    CharacterCardMiniSlot _newSlot;
    VisualElement _dragDropContainer;
    CharacterCardMini _draggedCard;

    List<CharacterCardMiniSlot> _allSlots = new();

    public void Initialize(VisualElement root, VisualElement cardContainer)
    {
        _root = root;
        _cardContainer = cardContainer;

        List<VisualElement> slots = root.Query(className: "character-card-mini-slot__main").ToList();
        foreach (VisualElement item in slots)
        {
            CharacterCardMiniSlot slot = (CharacterCardMiniSlot)item;
            AddDraggableSlot(slot);
        }

        List<VisualElement> cards = root.Query(className: "character-card-mini__main").ToList();
        foreach (VisualElement item in cards)
        {
            CharacterCardMini card = (CharacterCardMini)item;
            card.RegisterCallback<PointerDownEvent>(OnCardPointerDown);
            card.OnLocked += OnCardLocked;
            card.OnUnlocked += OnCardUnlocked;
        }

        _dragDropContainer = new VisualElement();
        _dragDropContainer.AddToClassList("characterDragDropContainer");
        _root.Add(_dragDropContainer);

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    public void AddDraggableSlot(CharacterCardMiniSlot slot)
    {
        if (_allSlots.Contains(slot))
            return;
        if (slot.IsLocked)
            return;

        _allSlots.Add(slot);
        slot.OnCardAdded += OnCardAdded;
        slot.OnLocked += OnSlotLocked;
    }

    void OnCardPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        CharacterCardMini card = (CharacterCardMini)evt.currentTarget;
        if (card.IsLocked)
            return;

        CharacterCardMiniSlot slotVisual = null;
        if (card.parent is CharacterCardMiniSlot)
        {
            slotVisual = (CharacterCardMiniSlot)card.parent;
            slotVisual.RemoveCard();
        }

        StartCardDrag(evt.position, slotVisual, card);
    }

    void StartCardDrag(Vector2 position, CharacterCardMiniSlot originalSlot, CharacterCardMini draggedCard)
    {
        _draggedCard = draggedCard;
        _draggedCard.style.top = 0;
        _draggedCard.style.left = 0;

        //Set tracking variables
        _isDragging = true;
        _originalSlot = originalSlot;
        //Set the new position
        _dragDropContainer.BringToFront();
        _dragDropContainer.style.top = position.y - _dragDropContainer.layout.height / 2;
        _dragDropContainer.style.left = position.x - _dragDropContainer.layout.width / 2;
        //Set the image
        _dragDropContainer.Add(_draggedCard);
        //Flip the visibility on
        _dragDropContainer.style.visibility = Visibility.Visible;
    }

    void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!_isDragging)
            return;

        if (_draggedCard != null)
        {
            //Set the new position
            _dragDropContainer.style.top = evt.position.y - _dragDropContainer.layout.height / 2;
            _dragDropContainer.style.left = evt.position.x - _dragDropContainer.layout.width / 2;
        }
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging)
            return;

        if (_draggedCard != null)
            HandleCardPointerUp();
    }

    void HandleCardPointerUp()
    {
        //Check to see if they are dropping the ghost icon over any inventory slots.
        IEnumerable<CharacterCardMiniSlot> slots = _allSlots.Where(x =>
               x.worldBound.Overlaps(_dragDropContainer.worldBound));

        //Didn't find any (dragged off the window)
        if (slots.Count() == 0)
        {
            _cardContainer.Add(_draggedCard);
            _draggedCard.style.top = _dragDropContainer.style.top.value.value - _cardContainer.worldBound.y;
            _draggedCard.style.left = _dragDropContainer.style.left;
            _draggedCard.Character.UpdateDeskPosition(new Vector2(_draggedCard.style.left.value.value,
                                                              _draggedCard.style.top.value.value));

            DragCleanUp();
            return;
        }

        //Found at least one
        _newSlot = slots.OrderBy(x => Vector2.Distance
           (x.worldBound.position, _dragDropContainer.worldBound.position)).First();

        if (_newSlot.Card != null)
        {
            CharacterCardMini copy = _newSlot.Card;
            _newSlot.RemoveCard();
            _newSlot.AddCard(_draggedCard);

            ReturnCardToContainer(copy);

            DragCleanUp();
            return;
        }

        //Set the new slot with the data
        _newSlot.AddCard(_draggedCard);

        DragCleanUp();
    }

    void ReturnCardToContainer(CharacterCardMini card)
    {
        _cardContainer.Add(card);
        card.style.position = Position.Absolute;
        card.style.left = _newSlot.worldBound.xMin - _dragDropContainer.layout.height / 2;
        card.style.top = _newSlot.worldBound.yMin - _dragDropContainer.layout.height / 2;
        int endLeft = Mathf.CeilToInt(_newSlot.worldBound.xMin) + Random.Range(-200, 200);
        int endTop = Mathf.CeilToInt(_newSlot.worldBound.yMin) + Random.Range(-200, 200);
        DOTween.To(() => card.style.left.value.value, x => card.style.left = x, endLeft, 0.5f);
        DOTween.To(() => card.style.top.value.value, x => card.style.top = x, endTop, 0.5f);

        card.Character.UpdateDeskPosition(new Vector2(endLeft, endTop));
    }

    protected virtual void DragCleanUp()
    {
        //Clear dragging related visuals and data
        _isDragging = false;

        _originalSlot = null;
        _draggedCard = null;

        _dragDropContainer.Clear();
        _dragDropContainer.style.visibility = Visibility.Hidden;
    }

    void OnCardAdded(CharacterCardMini card) { card.RegisterCallback<PointerDownEvent>(OnCardPointerDown); }

    public void OnSlotLocked(CharacterCardMiniSlot slot) { _allSlots.Remove(slot); }

    public void OnCardLocked(CharacterCardMini card) { card.UnregisterCallback<PointerDownEvent>(OnCardPointerDown); }
    public void OnCardUnlocked(CharacterCardMini card) { card.RegisterCallback<PointerDownEvent>(OnCardPointerDown); }

    public void RemoveDragContainer()
    {
        if (_dragDropContainer != null)
        {
            _root.Remove(_dragDropContainer);
            _dragDropContainer = null;
        }
    }
}

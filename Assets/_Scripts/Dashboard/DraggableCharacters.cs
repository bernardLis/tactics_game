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
    public bool IsDragging { get; private set; }

    CharacterCardMiniSlot _originalSlot;
    CharacterCardMiniSlot _newSlot;
    VisualElement _dragDropContainer;
    CharacterCardMini _draggedCard;

    List<CharacterCardMiniSlot> _allSlots = new();

    const string _ussDragDropContainer = "dashboard__character-drag-drop-container";
    const string _ussCardMiniSlot = "character-card-mini-slot__main";

    public void Initialize(VisualElement root, VisualElement cardContainer)
    {
        _root = root;
        _cardContainer = cardContainer;

        _allSlots = new();
        List<VisualElement> slots = root.Query(className: _ussCardMiniSlot).ToList();
        foreach (VisualElement item in slots)
            AddDraggableSlot((CharacterCardMiniSlot)item);

        _dragDropContainer = new VisualElement();
        _dragDropContainer.AddToClassList(_ussDragDropContainer);
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

    public void AddDraggableCard(CharacterCardMini card)
    {
        card.RegisterCallback<PointerDownEvent>(OnCardPointerDown);
        card.OnLocked += OnCardLocked;
        card.OnUnlocked += OnCardUnlocked;
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
            if (slotVisual.IsLocked)
                return;
            slotVisual.RemoveCard();
        }

        StartCardDrag(evt.position, slotVisual, card);
    }

    public void StartCardDrag(Vector2 position, CharacterCardMiniSlot originalSlot, CharacterCardMini draggedCard)
    {
        _draggedCard = draggedCard;
        _draggedCard.PickedUp();
        _draggedCard.style.position = Position.Absolute;
        _draggedCard.style.top = 0;
        _draggedCard.style.left = 0;

        //Set tracking variables
        IsDragging = true;
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
        if (!IsDragging)
            return;

        if (_draggedCard == null)
            return;

        //Set the new position
        _dragDropContainer.style.top = evt.position.y - _dragDropContainer.layout.height / 2;
        _dragDropContainer.style.left = evt.position.x - _dragDropContainer.layout.width / 2;
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (!IsDragging)
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
            SetDraggedCardPosition(new Vector2(_dragDropContainer.style.left.value.value,
                _dragDropContainer.style.top.value.value - _cardContainer.worldBound.y));

            DragCleanUp();
            return;
        }

        //Found at least one
        _newSlot = slots.OrderBy(x => Vector2.Distance
           (x.worldBound.position, _dragDropContainer.worldBound.position)).First();
        if (_draggedCard.Character.IsUnavailable)
        {
            ReturnCardToContainer(_draggedCard);
            DragCleanUp();
            return;
        }

        if (!_newSlot.IsSlotEmpty())
        {
            ReturnCardToContainer(_newSlot.Card);
            _newSlot.RemoveCard();
            _newSlot.AddCard(_draggedCard);
            DragCleanUp();
            return;
        }
        _newSlot.AddCard(_draggedCard);
        DragCleanUp();
    }

    void SetDraggedCardPosition(Vector2 newPos)
    {
        _draggedCard.style.left = newPos.x;
        _draggedCard.style.top = newPos.y;
        _draggedCard.Character.UpdateDeskPosition(newPos);
    }

    void ReturnCardToContainer(CharacterCardMini card)
    {
        _cardContainer.Add(card);
        card.style.position = Position.Absolute;
        card.style.left = _newSlot.worldBound.xMin - _dragDropContainer.layout.height / 2;
        card.style.top = _newSlot.worldBound.yMin - _dragDropContainer.layout.height / 2;
        int endLeft = Mathf.CeilToInt(_newSlot.worldBound.xMin) + Random.Range(-50, 50);
        int endTop = Mathf.CeilToInt(_newSlot.worldBound.yMin) + Random.Range(-50, 50);

        // when card is shaking and you grab it it behaves weirdly.
        card.UnregisterCallback<PointerDownEvent>(OnCardPointerDown);
        DOTween.To(() => card.style.left.value.value, x => card.style.left = x, endLeft, 0.5f)
                .SetEase(Ease.OutElastic);
        DOTween.To(() => card.style.top.value.value, x => card.style.top = x, endTop, 0.5f)
                .SetEase(Ease.OutElastic);
        card.RegisterCallback<PointerDownEvent>(OnCardPointerDown);

        card.Character.UpdateDeskPosition(new Vector2(endLeft, endTop));
    }

    protected virtual void DragCleanUp()
    {
        //Clear dragging related visuals and data
        IsDragging = false;
        _draggedCard.Dropped();

        _newSlot = null;
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

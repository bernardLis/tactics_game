using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class DraggableCharacters : MonoBehaviour
{
    VisualElement _root;


    // drag & drop
    // https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
    bool _isDragging;

    // Item drag & drop
    CharacterCardMiniSlot _originalSlot;
    CharacterCardMiniSlot _newSlot;
    VisualElement _dragDropContainer;
    CharacterCardMini _draggedCard;

    List<CharacterCardMiniSlot> _allSlots = new();

    public void Initialize(VisualElement root)
    {
        _root = root;

        List<VisualElement> elements = root.Query(className: "characterCardMiniSlot").ToList();
        foreach (VisualElement item in elements)
        {
            CharacterCardMiniSlot i = (CharacterCardMiniSlot)item;
            _allSlots.Add(i);

            if (i.Card != null)
                i.Card.RegisterCallback<PointerDownEvent>(OnCardPointerDown);
        }
        Debug.Log($"card mini elements: {elements.Count}");


        _dragDropContainer = new VisualElement();
        _dragDropContainer.AddToClassList("characterDragDropContainer");
        _root.Add(_dragDropContainer);

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }


    void OnCardPointerDown(PointerDownEvent evt)
    {

        if (evt.button != 0)
            return;

        CharacterCardMini card = (CharacterCardMini)evt.currentTarget;
        CharacterCardMiniSlot slotVisual = (CharacterCardMiniSlot)card.parent;
        slotVisual.RemoveCard();

        StartCardDrag(evt.position, slotVisual, card);
    }


    void StartCardDrag(Vector2 position, CharacterCardMiniSlot originalSlot, CharacterCardMini draggedCard)
    {
        _draggedCard = draggedCard;

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
            _originalSlot.AddCard(_draggedCard);
            DragCleanUp();
            return;
        }

        //Found at least one
        _newSlot = slots.OrderBy(x => Vector2.Distance
           (x.worldBound.position, _dragDropContainer.worldBound.position)).First();

        if (_newSlot.Card != null)
        {
            _originalSlot.AddCard(_draggedCard);
            DragCleanUp();
            return;
        }

        //Set the new inventory slot with the data
        _newSlot.AddCard(_draggedCard);

        DragCleanUp();
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

}

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

    public void Initialize(VisualElement root, VisualElement itemContainer)
    {
        _gameManager = GameManager.Instance;

        _root = root;
        _itemContainer = itemContainer;

        _dragDropContainer = new VisualElement();
        _dragDropContainer.AddToClassList("itemDragDropContainer");
        _root.Add(_dragDropContainer);

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    public void AddDraggableItem(ItemElement ItemElement)
    {
        ItemElement.RegisterCallback<PointerDownEvent>(OnItemPointerDown);
    }

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
        // _draggedItem.PickedUp();
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
        List<VisualElement> reportElements = _root.Query(className: "report__main").ToList();
        IEnumerable<VisualElement> overlappingReports = reportElements.Where(x =>
                    x.worldBound.Overlaps(_dragDropContainer.worldBound));

        // drag item from shop to desk to buy it
        if (_draggedItem.IsShop && overlappingReports.Count() == 0)
        {
            _itemContainer.Add(_draggedItem);
            SetDraggedItemPosition(new Vector2(_dragDropContainer.style.left.value.value,
                    _dragDropContainer.style.top.value.value - _itemContainer.worldBound.y));
            _gameManager.AddItemToPouch(_draggedItem.Item);
            _draggedItem.ItemBought();
            DragCleanUp();
            return;
        }
        // when you change your mind when buying
        if (_draggedItem.IsShop && overlappingReports.Count() != 0)
        {
            _draggedItem.style.top = 0;
            _draggedItem.style.left = 0;
            _draggedItem.style.position = Position.Relative;
            _originalSlot.AddItem(_draggedItem);
            DragCleanUp();
            return;
        }

        // drag item from desk to character to add it to character
        List<VisualElement> characterCards = _root.Query(className: "character-card-mini__main").ToList();
        IEnumerable<VisualElement> overlappingCards = characterCards.Where(x =>
                    x.worldBound.Overlaps(_dragDropContainer.worldBound));
        if (overlappingCards.Count() != 0)
        {
            VisualElement closesEl = overlappingCards.OrderBy(x => Vector2.Distance
                                     (x.worldBound.position, _dragDropContainer.worldBound.position)).First();
            CharacterCardMini closestCard = (CharacterCardMini)closesEl;
            if (closestCard.Character.CanTakeAnotherItem())
            {
                closestCard.Character.AddItem(_draggedItem.Item);
                _gameManager.RemoveItemFromPouch(_draggedItem.Item);
                DragCleanUp();
                return;
            }

            ShakeReturnItemToContainer(_draggedItem);
            DragCleanUp();
            return;
        }

        // move item around the desk - update & remember position
        _itemContainer.Add(_draggedItem);
        SetDraggedItemPosition(new Vector2(_dragDropContainer.style.left.value.value,
             _dragDropContainer.style.top.value.value - _itemContainer.worldBound.y));
        _gameManager.SaveJsonData();
        DragCleanUp();

        // drag item from desk to sell shop to sell it

    }

    void SetDraggedItemPosition(Vector2 newPos)
    {
        _draggedItem.style.left = newPos.x;
        _draggedItem.style.top = newPos.y;
        _draggedItem.Item.UpdateDeskPosition(newPos);
    }

    async void ShakeReturnItemToContainer(ItemElement itemElement)
    {
        _itemContainer.Add(itemElement);
        itemElement.style.position = Position.Absolute;
        itemElement.style.left = _dragDropContainer.worldBound.xMin;
        itemElement.style.top = _dragDropContainer.worldBound.yMin - _itemContainer.worldBound.y; ;
        int endLeft = Mathf.CeilToInt(_dragDropContainer.worldBound.xMin) + Random.Range(-50, 50);
        int endTop = Mathf.CeilToInt(_dragDropContainer.worldBound.yMin) + Random.Range(-50, 50);

        // when item is shaking and you grab it it behaves weirdly.
        itemElement.UnregisterCallback<PointerDownEvent>(OnItemPointerDown);
        DOTween.To(() => itemElement.style.left.value.value, x => itemElement.style.left = x, endLeft, 0.5f)
                .SetEase(Ease.OutElastic);
        await DOTween.To(() => itemElement.style.top.value.value, x => itemElement.style.top = x, endTop, 0.5f)
                .SetEase(Ease.OutElastic).AsyncWaitForCompletion();
        itemElement.RegisterCallback<PointerDownEvent>(OnItemPointerDown);

        itemElement.Item.UpdateDeskPosition(new Vector2(endLeft, endTop));
    }

    void DragCleanUp()
    {
        //Clear dragging related visuals and data
        _isDragging = false;
        // _draggedCard.Dropped();

        _originalSlot = null;
        _draggedItem = null;

        _dragDropContainer.Clear();
        _dragDropContainer.style.visibility = Visibility.Hidden;
    }

}

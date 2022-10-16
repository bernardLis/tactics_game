using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIDraggables : MonoBehaviour
{
    protected GameManager _gameManager;
    protected List<CharacterCardVisualExtended> _characterCards = new();

    protected VisualElement _root;

    // drag & drop
    // https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
    protected bool _isDragging;

    // Item drag & drop
    protected ItemSlotVisual _originalItemSlot;
    protected ItemSlotVisual _newItemSlot;
    protected VisualElement _itemDragDropContainer;
    protected ItemVisual _draggedItem;

    protected List<ItemSlotVisual> _allPlayerItemSlotVisuals = new();
    protected List<ItemSlotVisual> _playerPouchItemSlotVisuals = new();


    // Ability drag & drop
    protected AbilitySlotVisual _originalAbilitySlot;
    protected AbilitySlotVisual _newAbilitySlot;
    protected VisualElement _abilityDragDropContainer;
    protected AbilityButton _draggedAbility;

    protected List<AbilitySlotVisual> _allPlayerAbilitySlotVisuals = new();
    protected List<AbilitySlotVisual> _playerPouchAbilitySlotVisuals = new();

    bool _wasInitialized;

    public virtual void Initialize(VisualElement root)
    {
        _wasInitialized = true;

        _gameManager = GameManager.Instance;

        _allPlayerItemSlotVisuals = new();
        _playerPouchItemSlotVisuals = new();

        _allPlayerAbilitySlotVisuals = new();
        _playerPouchAbilitySlotVisuals = new();

        _root = root;
        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);

        _itemDragDropContainer = new VisualElement();
        _itemDragDropContainer.AddToClassList("itemDragDropContainer");
        _root.Add(_itemDragDropContainer);

        _abilityDragDropContainer = new VisualElement();
        _abilityDragDropContainer.AddToClassList("abilityDragDropContainer");
        _root.Add(_abilityDragDropContainer);
    }

    public VisualElement CreateCharacterCards(List<Character> troops)
    {
        // TODO: probably make it a scroll view
        VisualElement container = new();

        foreach (Character character in troops)
        {
            CharacterCardVisualExtended card = new CharacterCardVisualExtended(character);
            _characterCards.Add(card);
            container.Add(card);

            // allow moving character items
            foreach (ItemVisual item in card.ItemVisuals)
                item.RegisterCallback<PointerDownEvent>(OnItemPointerDown);

            foreach (ItemSlotVisual item in card.ItemSlots)
                _allPlayerItemSlotVisuals.Add(item);

            // allow moving character abilities
            foreach (AbilityButton ability in card.AbilityButtons)
                ability.RegisterCallback<PointerDownEvent>(OnAbilityPointerDown);

            foreach (AbilitySlotVisual slot in card.AbilitySlots)
                _allPlayerAbilitySlotVisuals.Add(slot);
        }

        return container;
    }

    public VisualElement CreateItemPouch()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.flexWrap = Wrap.Wrap;

        // TODO: probably make it a scroll view
        for (int i = 0; i < _gameManager.PlayerItemPouch.Count + 10; i++)
        {
            ItemSlotVisual slot = new ItemSlotVisual();
            container.Add(slot);
            _allPlayerItemSlotVisuals.Add(slot);
            _playerPouchItemSlotVisuals.Add(slot);
        }
        for (int i = 0; i < _gameManager.PlayerItemPouch.Count; i++)
        {
            ItemVisual itemVisual = new(_gameManager.PlayerItemPouch[i]);
            _playerPouchItemSlotVisuals[i].AddItem(itemVisual);
            itemVisual.RegisterCallback<PointerDownEvent>(OnItemPointerDown);
        }

        return container;
    }

    public VisualElement CreateAbilityPouch()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.flexWrap = Wrap.Wrap;

        // TODO: probably make it a scroll view
        //abilities
        for (int i = 0; i < _gameManager.PlayerItemPouch.Count + 10; i++)
        {
            AbilitySlotVisual slot = new AbilitySlotVisual();
            container.Add(slot);
            _allPlayerAbilitySlotVisuals.Add(slot);
            _playerPouchAbilitySlotVisuals.Add(slot);
        }

        for (int i = 0; i < _gameManager.PlayerAbilityPouch.Count; i++)
        {
            AbilityButton abilityButton = new(_gameManager.PlayerAbilityPouch[i], null);
            _playerPouchAbilitySlotVisuals[i].AddButton(abilityButton);
            abilityButton.RegisterCallback<PointerDownEvent>(OnAbilityPointerDown);
        }

        return container;
    }

    public void ClearDraggables()
    {
        if (!_wasInitialized)
            return;

        if (_itemDragDropContainer != null)
            _root.Remove(_itemDragDropContainer);
        if (_abilityDragDropContainer != null)
            _root.Remove(_abilityDragDropContainer);

        _root.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.UnregisterCallback<PointerUpEvent>(OnPointerUp);

        _wasInitialized = false;
    }

    protected virtual void OnItemPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        ItemVisual itemVisual = (ItemVisual)evt.target;
        ItemSlotVisual itemSlotVisual = (ItemSlotVisual)itemVisual.parent;
        itemSlotVisual.RemoveItem();

        StartItemDrag(evt.position, itemSlotVisual, itemVisual);
    }

    protected virtual void OnAbilityPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        AbilityButton abilityButton = (AbilityButton)evt.currentTarget;
        AbilitySlotVisual slotVisual = (AbilitySlotVisual)abilityButton.parent;
        slotVisual.RemoveButton();

        StartAbilityDrag(evt.position, slotVisual, abilityButton);
    }

    //drag & drop
    protected void StartItemDrag(Vector2 position, ItemSlotVisual originalSlot, ItemVisual draggedItem)
    {
        foreach (ItemSlotVisual slot in _allPlayerItemSlotVisuals)
            if (slot.ItemVisual == null)
                slot.Highlight();

        _draggedItem = draggedItem;

        //Set tracking variables
        _isDragging = true;
        _originalItemSlot = originalSlot;
        //Set the new position
        _itemDragDropContainer.BringToFront();
        _itemDragDropContainer.style.top = position.y - _itemDragDropContainer.layout.height / 2;
        _itemDragDropContainer.style.left = position.x - _itemDragDropContainer.layout.width / 2;
        //Set the image
        _itemDragDropContainer.Add(draggedItem);
        //Flip the visibility on
        _itemDragDropContainer.style.visibility = Visibility.Visible;
    }

    protected void StartAbilityDrag(Vector2 position, AbilitySlotVisual originalSlot, AbilityButton draggedAbility)
    {
        _draggedAbility = draggedAbility;

        //Set tracking variables
        _isDragging = true;
        _originalAbilitySlot = originalSlot;
        //Set the new position
        _abilityDragDropContainer.BringToFront();
        _abilityDragDropContainer.style.top = position.y - _abilityDragDropContainer.layout.height / 2;
        _abilityDragDropContainer.style.left = position.x - _abilityDragDropContainer.layout.width / 2;
        //Set the image
        _abilityDragDropContainer.Add(_draggedAbility);
        //Flip the visibility on
        _abilityDragDropContainer.style.visibility = Visibility.Visible;
    }

    protected void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!_isDragging)
            return;

        if (_draggedItem != null)
        {
            //Set the new position
            _itemDragDropContainer.style.top = evt.position.y - _itemDragDropContainer.layout.height / 2;
            _itemDragDropContainer.style.left = evt.position.x - _itemDragDropContainer.layout.width / 2;
        }

        if (_draggedAbility != null)
        {
            //Set the new position
            _abilityDragDropContainer.style.top = evt.position.y - _abilityDragDropContainer.layout.height / 2;
            _abilityDragDropContainer.style.left = evt.position.x - _abilityDragDropContainer.layout.width / 2;
        }
    }

    protected virtual void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging)
            return;

        if (_draggedItem != null)
            HandleItemPointerUp();

        if (_draggedAbility != null)
            HandleAbilityPointerUp();
    }

    protected virtual void HandleItemPointerUp()
    {
        //Check to see if they are dropping the ghost icon over any inventory slots.
        IEnumerable<ItemSlotVisual> slots = _allPlayerItemSlotVisuals.Where(x =>
               x.worldBound.Overlaps(_itemDragDropContainer.worldBound));

        //Didn't find any (dragged off the window)
        if (slots.Count() == 0)
        {
            _originalItemSlot.AddItem(_draggedItem);
            DragCleanUp();
            return;
        }

        //Found at least one
        _newItemSlot = slots.OrderBy(x => Vector2.Distance
           (x.worldBound.position, _itemDragDropContainer.worldBound.position)).First();

        if (_newItemSlot.ItemVisual != null)
        {
            _originalItemSlot.AddItem(_draggedItem);
            DragCleanUp();
            return;
        }

        //Set the new inventory slot with the data
        _newItemSlot.AddItem(_draggedItem);
        ItemMoved();
        DragCleanUp();
    }

    protected virtual void ItemMoved()
    {
        // removing
        if (_originalItemSlot.Character != null)
            _originalItemSlot.Character.RemoveItem(_draggedItem.Item);
        if (_playerPouchItemSlotVisuals.Contains(_originalItemSlot))
            _gameManager.RemoveItemFromPouch(_draggedItem.Item);

        // adding
        if (_newItemSlot.Character != null)
            _newItemSlot.Character.AddItem(_draggedItem.Item);
        if (_playerPouchItemSlotVisuals.Contains(_newItemSlot))
            _gameManager.AddItemToPouch(_draggedItem.Item);

        foreach (CharacterCardVisualExtended card in _characterCards)
            card.Character.ResolveItems();
    }

    protected virtual void HandleAbilityPointerUp()
    {
        //Check to see if they are dropping the ghost icon over any inventory slots.
        IEnumerable<AbilitySlotVisual> slots = _allPlayerAbilitySlotVisuals.Where(x =>
               x.worldBound.Overlaps(_abilityDragDropContainer.worldBound));

        //Didn't find any (dragged off the window)
        if (slots.Count() == 0)
        {
            _originalAbilitySlot.AddButton(_draggedAbility);
            DragCleanUp();
            return;
        }

        //Found at least one
        _newAbilitySlot = slots.OrderBy(x => Vector2.Distance
           (x.worldBound.position, _abilityDragDropContainer.worldBound.position)).First();

        if (_newAbilitySlot.AbilityButton != null)
        {
            _originalAbilitySlot.AddButton(_draggedAbility);
            DragCleanUp();
            return;
        }

        //Set the new inventory slot with the data
        _newAbilitySlot.AddButton(_draggedAbility);

        AbilityMoved();
        DragCleanUp();
    }

    protected virtual void AbilityMoved()
    {
        if (_originalAbilitySlot.Character != null)
            _originalAbilitySlot.Character.RemoveAbility(_draggedAbility.Ability);
        if (_playerPouchAbilitySlotVisuals.Contains(_originalAbilitySlot))
            _gameManager.RemoveAbilityFromPouch(_draggedAbility.Ability);

        if (_newAbilitySlot.Character != null)
            _newAbilitySlot.Character.AddAbility(_draggedAbility.Ability);
        if (_playerPouchAbilitySlotVisuals.Contains(_newAbilitySlot))
            _gameManager.AddAbilityToPouch(_draggedAbility.Ability);

    }

    protected virtual void DragCleanUp()
    {
        foreach (ItemSlotVisual slot in _allPlayerItemSlotVisuals)
            slot.ClearHighlight();

        //Clear dragging related visuals and data
        _isDragging = false;

        _originalItemSlot = null;
        _draggedItem = null;

        _originalAbilitySlot = null;
        _draggedAbility = null;

        _itemDragDropContainer.Clear();
        _itemDragDropContainer.style.visibility = Visibility.Hidden;

        _abilityDragDropContainer.Clear();
        _abilityDragDropContainer.style.visibility = Visibility.Hidden;
    }

}

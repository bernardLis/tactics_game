using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class ViewTroopsScreen : FullScreenVisual
{
    RunManager _runManager;
    public event Action OnClose;

    // drag & drop
    // https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
    bool _isDragging;

    // Item drag & drop
    ItemSlotVisual _originalItemSlot;
    ItemSlotVisual _newItemSlot;
    VisualElement _itemDragDropContainer;
    ItemVisual _draggedItem;
    List<ItemSlotVisual> _allPlayerItemSlotVisuals = new();
    List<ItemSlotVisual> _playerPouchItemSlotVisuals = new();

    List<CharacterCardVisualExtended> _characterCards = new();

    // Ability drag & drop
    AbilitySlotVisual _originalAbilitySlot;
    AbilitySlotVisual _newAbilitySlot;
    VisualElement _abilityDragDropContainer;
    AbilityButton _draggedAbility;

    List<AbilitySlotVisual> _allPlayerAbilitySlotVisuals = new();
    List<AbilitySlotVisual> _playerPouchAbilitySlotVisuals = new();


    public ViewTroopsScreen(List<Character> troops, VisualElement root, bool enableNavigation = true)
    {
        style.flexDirection = FlexDirection.Column;
        _runManager = RunManager.Instance;
        Initialize(root, enableNavigation);

        InitializePouches();
        InitializeCharacters(troops);

        if (enableNavigation)
            AddBackButton();

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);

        _itemDragDropContainer = new VisualElement();
        _itemDragDropContainer.AddToClassList("itemDragDropContainer");
        Add(_itemDragDropContainer);

        _abilityDragDropContainer = new VisualElement();
        _abilityDragDropContainer.AddToClassList("abilityDragDropContainer");
        Add(_abilityDragDropContainer);
    }

    void InitializePouches()
    {
        VisualElement c = new();
        Add(c);

        Label txt = new Label("Inventory:");
        txt.AddToClassList("textPrimary");
        c.Add(txt);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        c.Add(container);

        //abilities
        for (int i = 0; i < 3; i++)
        {
            AbilitySlotVisual slot = new AbilitySlotVisual();
            container.Add(slot);
            _allPlayerAbilitySlotVisuals.Add(slot);
            _playerPouchAbilitySlotVisuals.Add(slot);
        }

        for (int i = 0; i < _runManager.PlayerAbilityPouch.Count; i++)
        {
            AbilityButton abilityButton = new(_runManager.PlayerAbilityPouch[i], null);
            _playerPouchAbilitySlotVisuals[i].AddButton(abilityButton);
            abilityButton.RegisterCallback<PointerDownEvent>(OnPlayerAbilityPointerDown);
        }

        //items
        for (int i = 0; i < 3; i++)
        {
            ItemSlotVisual slot = new ItemSlotVisual();
            container.Add(slot);
            _allPlayerItemSlotVisuals.Add(slot);
            _playerPouchItemSlotVisuals.Add(slot);
        }
        for (int i = 0; i < _runManager.PlayerItemPouch.Count; i++)
        {
            ItemVisual itemVisual = new(_runManager.PlayerItemPouch[i]);
            _playerPouchItemSlotVisuals[i].AddItem(itemVisual);
            itemVisual.RegisterCallback<PointerDownEvent>(OnPlayerItemPointerDown);
        }
    }

    void InitializeCharacters(List<Character> troops)
    {

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        Add(container);

        foreach (Character character in troops)
        {
            CharacterCardVisualExtended card = new CharacterCardVisualExtended(character);
            _characterCards.Add(card);
            container.Add(card);

            // allow moving character items
            foreach (ItemVisual item in card.ItemVisuals)
                item.RegisterCallback<PointerDownEvent>(OnPlayerItemPointerDown);

            foreach (ItemSlotVisual item in card.ItemSlots)
                _allPlayerItemSlotVisuals.Add(item);

            // allow moving character abilities
            foreach (AbilityButton ability in card.AbilityButtons)
                ability.RegisterCallback<PointerDownEvent>(OnPlayerAbilityPointerDown);

            foreach (AbilitySlotVisual slot in card.AbilitySlots)
                _allPlayerAbilitySlotVisuals.Add(slot);
        }
    }

    void OnPlayerItemPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        ItemVisual itemVisual = (ItemVisual)evt.target;
        ItemSlotVisual itemSlotVisual = (ItemSlotVisual)itemVisual.parent;
        itemSlotVisual.RemoveItem();

        StartItemDrag(evt.position, itemSlotVisual, itemVisual);
    }

    void OnPlayerAbilityPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        AbilityButton abilityButton = (AbilityButton)evt.currentTarget;
        AbilitySlotVisual slotVisual = (AbilitySlotVisual)abilityButton.parent;
        slotVisual.RemoveButton();

        StartAbilityDrag(evt.position, slotVisual, abilityButton);
    }

    //drag & drop
    void StartItemDrag(Vector2 position, ItemSlotVisual originalSlot, ItemVisual draggedItem)
    {
        _draggedItem = draggedItem;

        //Set tracking variables
        _isDragging = true;
        _originalItemSlot = originalSlot;
        //Set the new position
        _itemDragDropContainer.style.top = position.y - _itemDragDropContainer.layout.height / 2;
        _itemDragDropContainer.style.left = position.x - _itemDragDropContainer.layout.width / 2;
        //Set the image
        _itemDragDropContainer.Add(draggedItem);
        //Flip the visibility on
        _itemDragDropContainer.style.visibility = Visibility.Visible;
    }

    void StartAbilityDrag(Vector2 position, AbilitySlotVisual originalSlot, AbilityButton draggedAbility)
    {
        _draggedAbility = draggedAbility;

        //Set tracking variables
        _isDragging = true;
        _originalAbilitySlot = originalSlot;
        //Set the new position
        _abilityDragDropContainer.style.top = position.y - _abilityDragDropContainer.layout.height / 2;
        _abilityDragDropContainer.style.left = position.x - _abilityDragDropContainer.layout.width / 2;
        //Set the image
        _abilityDragDropContainer.Add(_draggedAbility);
        //Flip the visibility on
        _abilityDragDropContainer.style.visibility = Visibility.Visible;
    }

    void OnPointerMove(PointerMoveEvent evt)
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

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging)
            return;

        if (_draggedItem != null)
            HandleItemPointerUp();

        if (_draggedAbility != null)
            HandleAbilityPointerUp();
    }

    void HandleItemPointerUp()
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

    void ItemMoved()
    {
        if (_originalItemSlot.Character != null)
            _originalItemSlot.Character.RemoveItem(_draggedItem.Item);
        else
            _runManager.RemoveItemFromPouch(_draggedItem.Item);

        if (_newItemSlot.Character != null)
            _newItemSlot.Character.AddItem(_draggedItem.Item);
        else
            _runManager.AddItemToPouch(_draggedItem.Item);

        foreach (CharacterCardVisualExtended card in _characterCards)
            card.Character.ResolveItems();
    }

    void HandleAbilityPointerUp()
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

    void AbilityMoved()
    {
        if (_originalAbilitySlot.Character != null)
            _originalAbilitySlot.Character.RemoveAbility(_draggedAbility.Ability);
        else
            _runManager.RemoveAbilityFromPouch(_draggedAbility.Ability);

        if (_newAbilitySlot.Character != null)
            _newAbilitySlot.Character.AddAbility(_draggedAbility.Ability);
        else
            _runManager.AddAbilityToPouch(_draggedAbility.Ability);

    }

    void DragCleanUp()
    {
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

    public override void Hide()
    {
        base.Hide();
        OnClose?.Invoke();
    }

}

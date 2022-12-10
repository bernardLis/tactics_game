using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class ScreenWithDraggables : FullScreenVisual
{
    GameManager _gameManager;

    // drag & drop
    // https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
    bool _isDragging;

    // Item drag & drop
    ItemSlotVisual _originalItemSlot;
    ItemSlotVisual _newItemSlot;
    VisualElement _itemDragDropContainer;
    ItemVisual _draggedItem;

    List<ItemSlotVisual> _rewardItemSlotVisuals = new();
    List<ItemSlotVisual> _allPlayerItemSlotVisuals = new();
    List<ItemSlotVisual> _playerPouchItemSlotVisuals = new();

    List<CharacterCard> _characterCards = new();

    // Ability drag & drop
    AbilitySlotVisual _originalAbilitySlot;
    AbilitySlotVisual _newAbilitySlot;
    VisualElement _abilityDragDropContainer;
    AbilityButton _draggedAbility;

    List<AbilitySlotVisual> _allPlayerAbilitySlotVisuals = new();
    List<AbilitySlotVisual> _playerPouchAbilitySlotVisuals = new();

    // gold
    GoldElement _goldElement;

    public ScreenWithDraggables(VisualElement root)
    {
        style.backgroundColor = Color.black;
        style.flexDirection = FlexDirection.Column;
        style.alignItems = Align.Center;
        _gameManager = GameManager.Instance;
        Initialize(root, false);

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);

        _itemDragDropContainer = new VisualElement();
        _itemDragDropContainer.AddToClassList("itemDragDropContainer");
        Add(_itemDragDropContainer);

        _abilityDragDropContainer = new VisualElement();
        _abilityDragDropContainer.AddToClassList("abilityDragDropContainer");
        Add(_abilityDragDropContainer);
    }

    public void AddElement(VisualElement el) { Add(el); }

    public ItemSlotVisual CreateDraggableItem(Item item, bool isDraggable = true)
    {
        ItemSlotVisual slot = new ItemSlotVisual();
        ItemVisual itemVisual = new(item);
        slot.AddItem(itemVisual);

        _rewardItemSlotVisuals.Add(slot);
        if (isDraggable)
            slot.ItemVisual.RegisterCallback<PointerDownEvent>(OnItemPointerDown);

        return slot;
    }

    public void UnlockItem(ItemVisual itemVisual)
    {
        itemVisual.RegisterCallback<PointerDownEvent>(OnItemPointerDown);
    }

    public bool AreAllRewardsTaken()
    {
        foreach (ItemSlotVisual slot in _rewardItemSlotVisuals)
            if (slot.ItemVisual != null)
            {
                Helpers.DisplayTextOnElement(this, slot, "Take me with you", Color.red);
                return false;
            }

        return true;
    }

    public VisualElement AddPouches()
    {
        VisualElement c = new();
        Add(c);

        Label txt = new Label("Inventory:");
        txt.AddToClassList("textPrimary");
        c.Add(txt);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        c.Add(container);

        GoldElement _goldElement = new(_gameManager.Gold);
        _gameManager.OnGoldChanged += _goldElement.ChangeAmount; // HERE: does it make a drama if I don't unsubcribe from it on destory?
        container.Add(_goldElement);

        //abilities
        for (int i = 0; i < 3; i++)
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

        //items
        for (int i = 0; i < 3; i++)
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

        return c;
    }

    public VisualElement AddCharacters(List<Character> troops)
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        Add(container);

        foreach (Character character in troops)
        {
            CharacterCard card = new CharacterCard(character);
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

    void OnItemPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        ItemVisual itemVisual = (ItemVisual)evt.target;
        ItemSlotVisual itemSlotVisual = (ItemSlotVisual)itemVisual.parent;
        itemSlotVisual.RemoveItem();

        StartItemDrag(evt.position, itemSlotVisual, itemVisual);
    }

    void OnAbilityPointerDown(PointerDownEvent evt)
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
        _itemDragDropContainer.BringToFront();
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
        _abilityDragDropContainer.BringToFront();
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
        // removing
        if (_originalItemSlot.Character != null)
            _originalItemSlot.Character.RemoveItem(_draggedItem.Item);
        if (_playerPouchItemSlotVisuals.Contains(_originalItemSlot))
            _gameManager.RemoveItemFromPouch(_draggedItem.Item);
        if (_rewardItemSlotVisuals.Contains(_originalItemSlot))
            _originalItemSlot.parent.Remove(_originalItemSlot);

        // adding
        if (_newItemSlot.Character != null)
        {
            _newItemSlot.Character.AddItem(_draggedItem.Item);
            Debug.Log($"Adding item to {_newItemSlot.Character}");
        }
        if (_playerPouchItemSlotVisuals.Contains(_newItemSlot))
        {
            _gameManager.AddItemToPouch(_draggedItem.Item);
            Debug.Log($"Adding item to pouch");

        }

        foreach (CharacterCard card in _characterCards)
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
        if (_playerPouchAbilitySlotVisuals.Contains(_originalAbilitySlot))
            _gameManager.RemoveAbilityFromPouch(_draggedAbility.Ability);

        if (_newAbilitySlot.Character != null)
            _newAbilitySlot.Character.AddAbility(_draggedAbility.Ability);
        if (_playerPouchAbilitySlotVisuals.Contains(_newAbilitySlot))
            _gameManager.AddAbilityToPouch(_draggedAbility.Ability);

        _gameManager.SaveJsonData();
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



}

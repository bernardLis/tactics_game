using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;
using System.Threading.Tasks;

public class ShopManager : UIDraggables
{
    GameManager _gameManager;

    VisualElement _shopContainer;
    VisualElement _shopItemContainer;
    VisualElement _shopAbilityContainer;

    VisualElement _shopSellContainer;
    Label _sellItemValueTooltip;
    VisualElement _shopRerollContainer;

    VisualElement _characterCardsContainer;
    VisualElement _playerItemPouch;
    VisualElement _playerAbilityPouch;

    Label _shopPlayerGoldAmount;
    GoldElement _goldElement;

    bool _wasVisited;


    // Item drag & drop
    bool _buyingItem;

    // Ability drag & drop
    bool _buyingAbility;

    void Awake()
    {
        _gameManager = GameManager.Instance;
        _runManager = RunManager.Instance;
        _runManager.OnGoldChanged += OnGoldChanged;

        _root = GetComponent<UIDocument>().rootVisualElement;

        _shopContainer = _root.Q<VisualElement>("shop");
        _shopItemContainer = _root.Q<VisualElement>("shopItemContainer");
        _shopAbilityContainer = _root.Q<VisualElement>("shopAbilityContainer");
        _shopSellContainer = _root.Q<VisualElement>("shopSellContainer");
        _sellItemValueTooltip = _root.Q<Label>("sellItemValueTooltip");

        _shopRerollContainer = _root.Q<VisualElement>("shopRerollContainer");
        MyButton rerollButton = new MyButton("Reroll Shop  price: 1", "menuButton", Reroll);
        rerollButton.AddToClassList("rerollButton");
        _shopRerollContainer.Add(rerollButton);

        _shopPlayerGoldAmount = _root.Q<Label>("shopPlayerGoldAmount");
        _goldElement = new GoldElement(_runManager.Gold);
        _shopPlayerGoldAmount.Add(_goldElement);

        VisualElement pouchContainer = _root.Q<VisualElement>("pouchContainer");
        pouchContainer.style.alignSelf = Align.FlexStart;
        _playerItemPouch = _root.Q<VisualElement>("playerItemPouch");
        _playerAbilityPouch = _root.Q<VisualElement>("playerAbilityPouch");

        _characterCardsContainer = _root.Q<VisualElement>("characterCardsContainer");

        MyButton shopBackButton = new MyButton("Back", "menuButton", Back);
        shopBackButton.style.position = Position.Absolute;
        shopBackButton.style.bottom = 0;
        _shopContainer.Add(shopBackButton);

        Initialize();

        //drag and drop
        _itemDragDropContainer = _root.Q<VisualElement>("itemDragDropContainer");
        _abilityDragDropContainer = _root.Q<VisualElement>("abilityDragDropContainer");

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnDestroy()
    {
        _runManager.OnGoldChanged -= OnGoldChanged;
    }

    void OnGoldChanged(int amount)
    {
        _goldElement.ChangeAmount(amount);
    }

    public void Initialize()
    {
        if (!_wasVisited)
        {
            PopulateShopItems();
            PopulateShopAbilities();
        }

        _wasVisited = true;

        _allPlayerItemSlotVisuals.Clear();
        PopulateCharacterCards();
        PopulatePlayerPouches();
    }

    void Reroll()
    {
        if (_runManager.Gold <= 0)
        {
            Helpers.DisplayTextOnElement(_root, _shopRerollContainer, "Insufficient funds", Color.red);
            return;
        }

        _runManager.ChangeGoldValue(-1);
        PopulateShopItems();
        PopulateShopAbilities();
    }

    void PopulateShopItems()
    {
        _shopItemContainer.Clear();
        for (int i = 0; i < 3; i++)
        {
            // so here I want 3 item slots that are filled with items 
            VisualElement container = new();
            container.style.alignItems = Align.Center;

            Item item = _gameManager.GameDatabase.GetRandomItem();
            ItemVisual itemVisual = new(item);
            ItemSlotVisual itemSlot = new();
            itemSlot.AddItem(itemVisual);

            //https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events-Handling.html
            itemVisual.RegisterCallback<PointerDownEvent>(OnShopItemPointerDown);

            container.Add(itemSlot);
            Label price = new Label("Price: " + item.Price);
            price.AddToClassList("textSecondary");
            container.Add(price); // TODO: coin logo instead of word "price" 

            _shopItemContainer.Add(container);
        }
    }

    void PopulateShopAbilities()
    {
        _shopAbilityContainer.Clear();
        for (int i = 0; i < 3; i++)
        {
            // so here I want 3 item slots that are filled with items 
            VisualElement container = new();
            container.style.alignItems = Align.Center;

            Ability ability = _gameManager.GameDatabase.GetRandomAbility();
            AbilityButton abilityButton = new(ability, null);
            AbilitySlotVisual abilitySlot = new();
            abilitySlot.AddButton(abilityButton);

            abilityButton.RegisterCallback<PointerDownEvent>(OnShopAbilityPointerDown);

            container.Add(abilitySlot);
            Label price = new Label("Price: " + ability.Price);
            price.AddToClassList("textSecondary");
            container.Add(price); // TODO: coin logo instead of word "price" 

            _shopAbilityContainer.Add(container);
        }
    }

    void PopulateCharacterCards()
    {
        _characterCards.Clear();
        _characterCardsContainer.Clear();
        List<Character> characters = new(_runManager.PlayerTroops);

        foreach (Character c in characters)
        {
            CharacterCardVisualExtended card = new(c);
            _characterCardsContainer.Add(card);
            _characterCards.Add(card);

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

    void PopulatePlayerPouches()
    {
        PopulatePlayerItemPouch();
        PopulatePlayerAbilityPouch();
    }

    void PopulatePlayerItemPouch()
    {
        _playerItemPouch.Clear();
        _playerPouchItemSlotVisuals.Clear();

        for (int i = 0; i < 3; i++)
        {
            ItemSlotVisual slot = new ItemSlotVisual();
            _playerItemPouch.Add(slot);
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

    void PopulatePlayerAbilityPouch()
    {
        _playerAbilityPouch.Clear();
        _playerPouchAbilitySlotVisuals.Clear();

        for (int i = 0; i < 3; i++)
        {
            AbilitySlotVisual slot = new AbilitySlotVisual();
            _playerAbilityPouch.Add(slot);
            _allPlayerAbilitySlotVisuals.Add(slot);
            _playerPouchAbilitySlotVisuals.Add(slot);
        }

        for (int i = 0; i < _runManager.PlayerAbilityPouch.Count; i++)
        {
            AbilityButton abilityButton = new(_runManager.PlayerAbilityPouch[i], null);
            _playerPouchAbilitySlotVisuals[i].AddButton(abilityButton);
            abilityButton.RegisterCallback<PointerDownEvent>(OnPlayerAbilityPointerDown);
        }

    }
    void Back()
    {
        VillageManager.Instance.ShowVillage();
        _gameManager.SaveJsonData();
    }

    void OnShopItemPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        ItemVisual itemVisual = (ItemVisual)evt.target;
        if (itemVisual.Item.Price > _runManager.Gold)
        {
            Helpers.DisplayTextOnElement(_root, itemVisual, "Insufficient funds", Color.red);
            return;
        }

        ItemSlotVisual itemSlotVisual = (ItemSlotVisual)itemVisual.parent;
        itemSlotVisual.RemoveItem();

        _buyingItem = true;
        StartItemDrag(evt.position, itemSlotVisual, itemVisual);
    }

    void OnPlayerItemPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        ItemVisual itemVisual = (ItemVisual)evt.target;
        ItemSlotVisual itemSlotVisual = (ItemSlotVisual)itemVisual.parent;
        itemSlotVisual.RemoveItem();

        _sellItemValueTooltip.text = $"Value: {itemVisual.Item.GetSellValue()}"; // TODO: money icon instead of "Value"

        StartItemDrag(evt.position, itemSlotVisual, itemVisual);
    }

    void OnShopAbilityPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        AbilityButton abilityButton = (AbilityButton)evt.currentTarget;
        if (abilityButton.Ability.Price > _runManager.Gold)
        {
            Helpers.DisplayTextOnElement(_root, abilityButton, "Insufficient funds", Color.red);
            return;
        }
        AbilitySlotVisual slotVisual = (AbilitySlotVisual)abilityButton.parent;
        slotVisual.RemoveButton();

        _buyingAbility = true;
        StartAbilityDrag(evt.position, slotVisual, abilityButton);
    }

    void OnPlayerAbilityPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        AbilityButton abilityButton = (AbilityButton)evt.currentTarget;
        AbilitySlotVisual slotVisual = (AbilitySlotVisual)abilityButton.parent;
        slotVisual.RemoveButton();

        _sellItemValueTooltip.text = $"Value: {1}"; // TODO: money icon instead of "Value"

        StartAbilityDrag(evt.position, slotVisual, abilityButton);
    }
    void HandleItemPointerUp()
    {
        if (_shopSellContainer.worldBound.Overlaps(_itemDragDropContainer.worldBound) && !_buyingItem)
        {
            ItemSold();
            DragCleanUp();
            return;
        }

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
        if (_buyingItem)
            ItemBought();
        else
            ItemMoved();
        DragCleanUp();
    }

    void ItemSold()
    {
        _runManager.ChangeGoldValue(_draggedItem.Item.GetSellValue());

        if (_originalItemSlot.Character != null)
            _originalItemSlot.Character.RemoveItem(_draggedItem.Item);
        else
            _runManager.RemoveItemFromPouch(_draggedItem.Item);
    }

    void ItemBought()
    {
        _runManager.ChangeGoldValue(-_draggedItem.Item.Price);

        if (_newItemSlot.Character != null)
            _newItemSlot.Character.AddItem(_draggedItem.Item);
        else
            _runManager.AddItemToPouch(_draggedItem.Item);

        // unregister buy pointer and register sell pointer
        _draggedItem.UnregisterCallback<PointerDownEvent>(OnShopItemPointerDown);
        _draggedItem.RegisterCallback<PointerDownEvent>(OnPlayerItemPointerDown);
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
        if (_shopSellContainer.worldBound.Overlaps(_abilityDragDropContainer.worldBound) && !_buyingAbility)
        {
            AbilitySold();
            DragCleanUp();
            return;
        }

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

        if (_buyingAbility)
            AbilityBought();
        else
            AbilityMoved();
        DragCleanUp();
    }

    void AbilitySold()
    {
        _runManager.ChangeGoldValue(1);

        if (_originalAbilitySlot.Character != null)
            _originalAbilitySlot.Character.RemoveAbility(_draggedAbility.Ability);
        else
            _runManager.RemoveAbilityFromPouch(_draggedAbility.Ability);
    }

    void AbilityBought()
    {
        _runManager.ChangeGoldValue(-_draggedAbility.Ability.Price);

        if (_newAbilitySlot.Character != null)
            _newAbilitySlot.Character.AddAbility(_draggedAbility.Ability);
        else
            _runManager.AddAbilityToPouch(_draggedAbility.Ability);

        // unregister buy pointer and register sell pointer
        _draggedAbility.UnregisterCallback<PointerDownEvent>(OnShopAbilityPointerDown);
        _draggedAbility.RegisterCallback<PointerDownEvent>(OnPlayerAbilityPointerDown);
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

        _buyingItem = false;
        _buyingAbility = false;

        _originalItemSlot = null;
        _draggedItem = null;

        _originalAbilitySlot = null;
        _draggedAbility = null;

        _itemDragDropContainer.Clear();
        _itemDragDropContainer.style.visibility = Visibility.Hidden;

        _abilityDragDropContainer.Clear();
        _abilityDragDropContainer.style.visibility = Visibility.Hidden;

        _sellItemValueTooltip.text = "";
    }
}

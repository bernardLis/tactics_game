using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;
using System.Threading.Tasks;

public class ShopManager : MonoBehaviour
{
    GameManager _gameManager;

    VisualElement _root;

    VisualElement _shopItemContainer;
    VisualElement _shopAbilityContainer;

    VisualElement _shopSellContainer;
    Label _sellItemValueTooltip;
    Button _shopRerollButton;

    VisualElement _characterCardsContainer;
    VisualElement _playerItemPouch;
    VisualElement _playerAbilityPouch;

    Label _shopPlayerGoldAmount;

    bool _wasVisited;

    // drag & drop
    // https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
    bool _isDragging;

    // Item drag & drop
    bool _buyingItem;
    ItemSlotVisual _originalItemSlot;
    ItemSlotVisual _newItemSlot;
    VisualElement _itemDragDropContainer;
    ItemVisual _draggedItem;
    List<ItemSlotVisual> _allPlayerItemSlotVisuals = new();
    List<ItemSlotVisual> _playerPouchItemSlotVisuals = new();

    // Ability drag & drop
    bool _buyingAbility;
    AbilitySlotVisual _originalAbilitySlot;
    AbilitySlotVisual _newAbilitySlot;
    VisualElement _abilityDragDropContainer;
    AbilityButton _draggedAbility;

    List<AbilitySlotVisual> _allPlayerAbilitySlotVisuals = new();
    List<AbilitySlotVisual> _playerPouchAbilitySlotVisuals = new();


    List<CharacterCardVisualShop> _characterCards = new();

    void Awake()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnGoldChanged += OnGoldChanged;

        _root = GetComponent<UIDocument>().rootVisualElement;

        _shopItemContainer = _root.Q<VisualElement>("shopItemContainer");
        _shopAbilityContainer = _root.Q<VisualElement>("shopAbilityContainer");
        _shopSellContainer = _root.Q<VisualElement>("shopSellContainer");
        _sellItemValueTooltip = _root.Q<Label>("sellItemValueTooltip");

        _shopRerollButton = _root.Q<Button>("shopRerollButton");
        _shopRerollButton.clickable.clicked += Reroll;


        _shopPlayerGoldAmount = _root.Q<Label>("shopPlayerGoldAmount");
        _shopPlayerGoldAmount.text = "" + _gameManager.Gold;

        VisualElement pouchContainer = _root.Q<VisualElement>("pouchContainer");
        pouchContainer.style.alignSelf = Align.FlexStart;
        _playerItemPouch = _root.Q<VisualElement>("playerItemPouch");
        _playerAbilityPouch = _root.Q<VisualElement>("playerAbilityPouch");

        _characterCardsContainer = _root.Q<VisualElement>("characterCardsContainer");

        Button shopBackButton = _root.Q<Button>("shopBackButton");
        shopBackButton.clickable.clicked += Back;

        Initialize();

        //drag and drop
        _itemDragDropContainer = _root.Q<VisualElement>("itemDragDropContainer");
        _abilityDragDropContainer = _root.Q<VisualElement>("abilityDragDropContainer");

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnDestroy()
    {
        _gameManager.OnGoldChanged -= OnGoldChanged;
    }

    void OnGoldChanged(int amount)
    {
        _shopPlayerGoldAmount.text = "" + amount;
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
        if (_gameManager.Gold <= 0)
        {
            DisplayText(_shopRerollButton, "Insufficient funds", Color.red);
            return;
        }

        _gameManager.ChangeGoldValue(-1);
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

            Item item = _gameManager.CharacterDatabase.GetRandomItem();
            ItemVisual itemVisual = new(item);
            ItemSlotVisual itemSlot = new(itemVisual);

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

            Ability ability = _gameManager.CharacterDatabase.GetRandomAbility();
            AbilityButton abilityButton = new(ability, null);
            AbilitySlotVisual abilitySlot = new(abilityButton);

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
        List<Character> characters = new(_gameManager.PlayerTroops);

        foreach (Character c in characters)
        {
            CharacterCardVisualShop card = new(c);
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

        for (int i = 0; i < _gameManager.PlayerItemPouch.Count; i++)
        {
            ItemVisual itemVisual = new(_gameManager.PlayerItemPouch[i]);
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

        for (int i = 0; i < _gameManager.PlayerAbilityPouch.Count; i++)
        {
            AbilityButton abilityButton = new(_gameManager.PlayerAbilityPouch[i], null);
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
        if (itemVisual.Item.Price > _gameManager.Gold)
        {
            DisplayText(itemVisual, "Insufficient funds", Color.red);
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
        if (abilityButton.Ability.Price > _gameManager.Gold)
        {
            DisplayText(abilityButton, "Insufficient funds", Color.red);
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
        _gameManager.ChangeGoldValue(_draggedItem.Item.GetSellValue());

        if (_originalItemSlot.Character != null)
            _originalItemSlot.Character.RemoveItem(_draggedItem.Item);
        else
            _gameManager.PlayerItemPouch.Remove(_draggedItem.Item);
    }

    void ItemBought()
    {
        _gameManager.ChangeGoldValue(-_draggedItem.Item.Price);

        if (_newItemSlot.Character != null)
            _newItemSlot.Character.AddItem(_draggedItem.Item);
        else
            _gameManager.PlayerItemPouch.Add(_draggedItem.Item);

        // unregister buy pointer and register sell pointer
        _draggedItem.UnregisterCallback<PointerDownEvent>(OnShopItemPointerDown);
        _draggedItem.RegisterCallback<PointerDownEvent>(OnPlayerItemPointerDown);
    }

    void ItemMoved()
    {
        if (_originalItemSlot.Character != null)
            _originalItemSlot.Character.RemoveItem(_draggedItem.Item);
        else
            _gameManager.PlayerItemPouch.Remove(_draggedItem.Item);

        if (_newItemSlot.Character != null)
            _newItemSlot.Character.AddItem(_draggedItem.Item);

        foreach (CharacterCardVisualShop card in _characterCards)
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
        _gameManager.ChangeGoldValue(1);

        if (_originalAbilitySlot.Character != null)
            _originalAbilitySlot.Character.RemoveAbility(_draggedAbility.Ability);
        else
            _gameManager.PlayerAbilityPouch.Remove(_draggedAbility.Ability);
    }

    void AbilityBought()
    {
        _gameManager.ChangeGoldValue(-_draggedAbility.Ability.Price);

        if (_newAbilitySlot.Character != null)
            _newAbilitySlot.Character.AddAbility(_draggedAbility.Ability);
        else
            _gameManager.PlayerAbilityPouch.Add(_draggedAbility.Ability);

        // unregister buy pointer and register sell pointer
        _draggedAbility.UnregisterCallback<PointerDownEvent>(OnShopAbilityPointerDown);
        _draggedAbility.RegisterCallback<PointerDownEvent>(OnPlayerAbilityPointerDown);
    }

    void AbilityMoved()
    {
        if (_originalAbilitySlot.Character != null)
            _originalAbilitySlot.Character.RemoveAbility(_draggedAbility.Ability);
        else
            _gameManager.PlayerAbilityPouch.Remove(_draggedAbility.Ability);

        if (_newAbilitySlot.Character != null)
            _newAbilitySlot.Character.AddAbility(_draggedAbility.Ability);
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

    async void DisplayText(VisualElement element, string text, Color color)
    {
        Label l = new Label(text);
        l.AddToClassList("textSecondary");
        l.style.color = color;
        l.style.position = Position.Absolute;
        l.style.left = element.worldBound.xMin - element.worldBound.width / 2;
        l.style.top = element.worldBound.yMax;

        _root.Add(l);
        float end = element.worldBound.yMin + 100;
        await DOTween.To(x => l.style.top = x, element.worldBound.yMax, end, 1).SetEase(Ease.OutSine).AsyncWaitForCompletion();
        await DOTween.To(x => l.style.opacity = x, 1, 0, 1).AsyncWaitForCompletion();
        _root.Remove(l);
    }
}

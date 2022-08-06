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
    bool _buying;
    ItemSlotVisual _originalSlot;
    ItemSlotVisual _newSlot;
    VisualElement _dragDropContainer;
    ItemVisual _draggedItem;

    List<ItemSlotVisual> _allPlayerItemSlotVisuals = new();
    List<ItemSlotVisual> _playerPouchItemSlotVisuals = new();

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

        _characterCardsContainer = _root.Q<VisualElement>("characterCardsContainer");

        _playerItemPouch = _root.Q<VisualElement>("playerItemPouch");
        _playerAbilityPouch = _root.Q<VisualElement>("playerAbilityPouch");

        _shopPlayerGoldAmount = _root.Q<Label>("shopPlayerGoldAmount");
        _shopPlayerGoldAmount.text = "" + _gameManager.Gold;

        Button shopBackButton = _root.Q<Button>("shopBackButton");
        shopBackButton.clickable.clicked += Back;

        Initialize();

        //drag and drop
        _dragDropContainer = _root.Q<VisualElement>("dragDropContainer");

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
            PopulateItems();
            PopulateAbilities();
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
        PopulateItems();
    }

    void PopulateItems()
    {
        _shopItemContainer.Clear();
        for (int i = 0; i < 3; i++)
        {
            // so here I want 3 item slots that are filled with items 
            VisualElement container = new();
            container.style.alignItems = Align.Center;

            Item item = _gameManager.CharacterDatabase.GetRandomItem();
            ItemVisual iv = new(item);
            ItemSlotVisual itemSlot = new(iv);

            //https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events-Handling.html
            iv.RegisterCallback<PointerDownEvent>(OnShopItemPointerDown);

            container.Add(itemSlot);
            Label price = new Label("Price: " + item.Price);
            price.AddToClassList("textSecondary");
            container.Add(price); // TODO: coin logo instead of word "price" 

            _shopItemContainer.Add(container);
        }
    }

    void PopulateAbilities()
    {
        _shopAbilityContainer.Clear();
        for (int i = 0; i < 3; i++)
        {
            // so here I want 3 item slots that are filled with items 
            VisualElement container = new();
            container.style.alignItems = Align.Center;

            Ability ability = _gameManager.CharacterDatabase.GetRandomAbility();
            AbilityButton abilityButton = new(ability, null, _root);
            AbilitySlotVisual abilitySlot = new(abilityButton);

            //https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events-Handling.html
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
            _playerPouchItemSlotVisuals[i].Add(itemVisual);
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
            //                        _allPlayerItemSlotVisuals.Add(slot);
            _playerPouchAbilitySlotVisuals.Add(slot);
        }

        for (int i = 0; i < _gameManager.PlayerAbilityPouch.Count; i++)
        {
            AbilityButton abilityButton = new(_gameManager.PlayerAbilityPouch[i], null, _root);
            _playerPouchAbilitySlotVisuals[i].Add(abilityButton);

            // abilityButton.RegisterCallback<PointerDownEvent>(OnPlayerItemPointerDown);
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
        itemSlotVisual.Remove(itemVisual);

        _buying = true;
        StartDrag(evt.position, itemSlotVisual, itemVisual);
    }

    void OnPlayerItemPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        ItemVisual itemVisual = (ItemVisual)evt.target;
        ItemSlotVisual itemSlotVisual = (ItemSlotVisual)itemVisual.parent;
        itemSlotVisual.Remove(itemVisual);

        _sellItemValueTooltip.text = $"Value: {itemVisual.Item.GetSellValue()}"; // TODO: money icon instead of "Value"

        StartDrag(evt.position, itemSlotVisual, itemVisual);
    }

    void OnShopAbilityPointerDown(PointerDownEvent evt)
    {
        Debug.Log($"shop ability pointer down");
    }


    //drag & drop
    public void StartDrag(Vector2 position, ItemSlotVisual originalSlot, ItemVisual draggedItem)
    {
        _draggedItem = draggedItem;

        //Set tracking variables
        _isDragging = true;
        _originalSlot = originalSlot;
        //Set the new position
        _dragDropContainer.style.top = position.y - _dragDropContainer.layout.height / 2;
        _dragDropContainer.style.left = position.x - _dragDropContainer.layout.width / 2;
        //Set the image
        _dragDropContainer.Add(draggedItem);
        //Flip the visibility on
        _dragDropContainer.style.visibility = Visibility.Visible;
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!_isDragging)
            return;

        //Set the new position
        _dragDropContainer.style.top = evt.position.y - _dragDropContainer.layout.height / 2;
        _dragDropContainer.style.left = evt.position.x - _dragDropContainer.layout.width / 2;
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging)
            return;

        if (_shopSellContainer.worldBound.Overlaps(_dragDropContainer.worldBound) && !_buying)
        {
            ItemSold();
            DragCleanUp();
            return;
        }

        //Check to see if they are dropping the ghost icon over any inventory slots.
        IEnumerable<ItemSlotVisual> slots = _allPlayerItemSlotVisuals.Where(x =>
               x.worldBound.Overlaps(_dragDropContainer.worldBound));

        //Didn't find any (dragged off the window)
        if (slots.Count() == 0)
        {
            _originalSlot.AddItem(_draggedItem);
            DragCleanUp();
            return;
        }

        //Found at least one
        _newSlot = slots.OrderBy(x => Vector2.Distance
           (x.worldBound.position, _dragDropContainer.worldBound.position)).First();
        //Set the new inventory slot with the data
        _newSlot.AddItem(_draggedItem);
        if (_buying)
            ItemBought();
        else
            ItemMoved();
        DragCleanUp();
    }

    void ItemSold()
    {
        _gameManager.ChangeGoldValue(_draggedItem.Item.GetSellValue());

        if (_originalSlot.Character != null)
            _originalSlot.Character.RemoveItem(_draggedItem.Item);
        else
            _gameManager.PlayerItemPouch.Remove(_draggedItem.Item);
    }

    void ItemBought()
    {
        _gameManager.ChangeGoldValue(-_draggedItem.Item.Price);

        if (_newSlot.Character != null)
            _newSlot.Character.AddItem(_draggedItem.Item);
        else
            _gameManager.PlayerItemPouch.Add(_draggedItem.Item);

        // unregister buy pointer and register sell pointer
        _draggedItem.UnregisterCallback<PointerDownEvent>(OnShopItemPointerDown);
        _draggedItem.RegisterCallback<PointerDownEvent>(OnPlayerItemPointerDown);
    }

    void ItemMoved()
    {
        if (_originalSlot.Character != null)
            _originalSlot.Character.RemoveItem(_draggedItem.Item);
        else
            _gameManager.PlayerItemPouch.Remove(_draggedItem.Item);

        if (_newSlot.Character != null)
            _newSlot.Character.AddItem(_draggedItem.Item);

        foreach (CharacterCardVisualShop card in _characterCards)
            card.Character.ResolveItems();
    }

    void DragCleanUp()
    {
        //Clear dragging related visuals and data
        _buying = false;
        _isDragging = false;
        _originalSlot = null;
        _draggedItem = null;
        _dragDropContainer.Clear();
        _dragDropContainer.style.visibility = Visibility.Hidden;

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

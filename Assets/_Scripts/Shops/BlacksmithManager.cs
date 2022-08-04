using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;
using System.Threading.Tasks;

public class BlacksmithManager : MonoBehaviour
{
    GameManager _gameManager;

    VisualElement _root;

    VisualElement _blacksmithItemContainer;

    VisualElement _blacksmithSellContainer;
    Label _sellItemValueTooltip;

    VisualElement _characterCardsContainer;
    VisualElement _playerItemPouch;
    Label _blacksmithPlayerObolsAmount;

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

    List<CharacterCardVisualShop> _characterCards = new();

    void Awake()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnObolsChanged += OnObolsChanged;

        _root = GetComponent<UIDocument>().rootVisualElement;

        _blacksmithItemContainer = _root.Q<VisualElement>("blacksmithItemContainer");
        _blacksmithSellContainer = _root.Q<VisualElement>("blacksmithSellContainer");
        _sellItemValueTooltip = _root.Q<Label>("sellItemValueTooltip");

        Button blacksmithRerollButton = _root.Q<Button>("blacksmithRerollButton");
        blacksmithRerollButton.clickable.clicked += Reroll;

        _characterCardsContainer = _root.Q<VisualElement>("characterCardsContainer");
        _playerItemPouch = _root.Q<VisualElement>("playerItemPouch");
        _blacksmithPlayerObolsAmount = _root.Q<Label>("blacksmithPlayerObolsAmount");
        _blacksmithPlayerObolsAmount.text = "" + _gameManager.Obols;

        Button blacksmithBackButton = _root.Q<Button>("blacksmithBackButton");
        blacksmithBackButton.clickable.clicked += Back;

        Initialize();

        //drag and drop
        _dragDropContainer = _root.Q<VisualElement>("dragDropContainer");

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnDestroy()
    {
        _gameManager.OnObolsChanged -= OnObolsChanged;
    }

    void OnObolsChanged(int amount)
    {
        _blacksmithPlayerObolsAmount.text = "" + amount;
    }

    public void Initialize()
    {
        if (!_wasVisited)
            PopulateItems();

        _wasVisited = true;

        _allPlayerItemSlotVisuals.Clear();
        _playerItemPouch.Clear();
        _characterCards.Clear();

        PopulateCharacterCards();
        PopulatePlayerPouch();
    }

    void Reroll()
    {
        if (_gameManager.Obols <= 0)
            return;
        _gameManager.ChangeObolsValue(-1);
        PopulateItems();
    }

    void PopulateItems()
    {
        _blacksmithItemContainer.Clear();
        for (int i = 0; i < 3; i++)
        {
            // so here I want 3 item slots that are filled with items 
            VisualElement container = new();
            container.style.alignItems = Align.Center;

            Item item = _gameManager.CharacterDatabase.GetRandomItem();
            ItemVisual iv = new(item);
            ItemSlotVisual itemSlot = new(iv);

            //https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events-Handling.html
            iv.RegisterCallback<PointerDownEvent>(OnBlacksmithItemPointerDown);

            container.Add(itemSlot);
            Label price = new Label("Price: " + item.Price);
            price.AddToClassList("textSecondary");
            container.Add(price); // TODO: coin logo instead of word "price" 

            _blacksmithItemContainer.Add(container);
        }
    }

    void PopulateCharacterCards()
    {
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

    void PopulatePlayerPouch()
    {
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

    void Back()
    {
        ShopManager.Instance.ShowShops();
        _gameManager.SaveJsonData();
    }

    void OnBlacksmithItemPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        ItemVisual itemVisual = (ItemVisual)evt.target;
        if (itemVisual.Item.Price > _gameManager.Obols)
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

        if (_blacksmithSellContainer.worldBound.Overlaps(_dragDropContainer.worldBound) && !_buying)
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
        _gameManager.ChangeObolsValue(_draggedItem.Item.GetSellValue());

        if (_originalSlot.Character != null)
            _originalSlot.Character.RemoveItem(_draggedItem.Item);
        else
            _gameManager.PlayerItemPouch.Remove(_draggedItem.Item);
    }

    void ItemBought()
    {
        _gameManager.ChangeObolsValue(-_draggedItem.Item.Price);

        if (_newSlot.Character != null)
            _newSlot.Character.AddItem(_draggedItem.Item);
        else
            _gameManager.PlayerItemPouch.Add(_draggedItem.Item);

        // unregister buy pointer and register sell pointer
        _draggedItem.UnregisterCallback<PointerDownEvent>(OnBlacksmithItemPointerDown);
        _draggedItem.RegisterCallback<PointerDownEvent>(OnPlayerItemPointerDown);
    }

    void ItemMoved()
    {
        if (_originalSlot.Character != null)
            _originalSlot.Character.RemoveItem(_draggedItem.Item);
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

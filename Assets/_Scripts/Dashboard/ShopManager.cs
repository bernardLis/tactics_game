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
    DashboardManager _dashboardManager;

    VisualElement _shopItemContainer;

    VisualElement _shopRerollContainer;
    GoldElement _rerollPriceGoldElement;

    VisualElement _shopSellContainer;
    VisualElement _sellItemValueTooltip;

    VisualElement _pouchContainer;

    List<Item> _shopItems = new();

    // Item drag & drop
    bool _buyingItem;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;

        _dashboardManager = GetComponent<DashboardManager>();
        _root = _dashboardManager.Root;
        _dashboardManager.OnShopClicked += OnShopClicked;

        ChooseShopItems();

        _shopItemContainer = _root.Q<VisualElement>("shopItemContainer");
        _shopRerollContainer = _root.Q<VisualElement>("shopRerollContainer");
        MyButton rerollButton = new MyButton("", "rerollButton", Reroll);
        _rerollPriceGoldElement = new(_gameManager.ShopRerollPrice);
        _gameManager.OnShopRerollPriceChanged += _rerollPriceGoldElement.ChangeAmount;

        _shopRerollContainer.Add(rerollButton);
        _shopRerollContainer.Add(_rerollPriceGoldElement);

        _shopSellContainer = _root.Q<VisualElement>("shopSellContainer");
        _sellItemValueTooltip = _root.Q<VisualElement>("sellItemValueTooltip");

        _pouchContainer = _root.Q<VisualElement>("pouchContainer");
    }

    void OnDayPassed(int dayNumber) { ChooseShopItems(); }

    void OnShopClicked() { Initialize(_root); }

    public override void Initialize(VisualElement root)
    {
        base.Initialize(root);

        DisplayShopItems();

        _pouchContainer.Clear();
        Label header = new Label("Item Pouch");
        header.AddToClassList("textPrimary");
        _pouchContainer.Add(header);
        _pouchContainer.Add(CreateItemPouch());
    }

    void ChooseShopItems()
    {
        _shopItems.Clear();
        for (int i = 0; i < 6; i++)
        {
            Item item = _gameManager.GameDatabase.GetRandomItem();
            _shopItems.Add(item);
        }
    }

    void Reroll()
    {
        if (_runManager.Gold < _gameManager.ShopRerollPrice)
        {
            Helpers.DisplayTextOnElement(_root, _shopRerollContainer, "Insufficient funds", Color.red);
            return;
        }
        int lastPrice = _gameManager.ShopRerollPrice;

        _runManager.ChangeGoldValue(-lastPrice);
        AudioManager.Instance.PlaySFX("DiceRoll", Vector3.zero);

        _gameManager.ChangeShopRerollPrice(lastPrice *= 2);

        ChooseShopItems();
        DisplayShopItems();
    }

    void DisplayShopItems()
    {
        _shopItemContainer.Clear();
        for (int i = 0; i < _shopItems.Count; i++)
        {
            // so here I want 3 item slots that are filled with items 
            VisualElement container = new();
            container.AddToClassList("shopItem");

            ItemVisual itemVisual = new(_shopItems[i]);
            ItemSlotVisual itemSlot = new();
            itemSlot.AddItem(itemVisual);

            //https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events-Handling.html
            itemVisual.RegisterCallback<PointerDownEvent>(OnShopItemPointerDown);

            container.Add(itemSlot);
            container.Add(new GoldElement(_shopItems[i].Price));

            _shopItemContainer.Add(container);
        }
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

    protected override void OnItemPointerDown(PointerDownEvent evt)
    {
        base.OnItemPointerDown(evt);

        if (_buyingItem)
            return;

        ItemVisual itemVisual = (ItemVisual)evt.target;
        _sellItemValueTooltip.Add(new GoldElement(itemVisual.Item.GetSellValue()));
    }

    protected override void HandleItemPointerUp()
    {
        // selling
        if (_shopSellContainer.worldBound.Overlaps(_itemDragDropContainer.worldBound) && !_buyingItem)
        {
            ItemSold();
            DragCleanUp();
            return;
        }

        base.HandleItemPointerUp();
        // buying
        if (_buyingItem)
            ItemBought();
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
        _shopItems.Remove(_draggedItem.Item);

        // TODO: does it double the item
        if (_newItemSlot.Character != null)
            _newItemSlot.Character.AddItem(_draggedItem.Item);
        else
            _runManager.AddItemToPouch(_draggedItem.Item);

        // unregister buy pointer and register sell pointer
        _draggedItem.UnregisterCallback<PointerDownEvent>(OnShopItemPointerDown);
        _draggedItem.RegisterCallback<PointerDownEvent>(OnItemPointerDown);
    }

    protected override void DragCleanUp()
    {
        base.DragCleanUp();

        _buyingItem = false;
        _sellItemValueTooltip.Clear();
    }
}

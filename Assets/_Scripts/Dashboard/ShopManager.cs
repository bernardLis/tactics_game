using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;
using System.Threading.Tasks;

public class ShopManager : UIDraggables
{
    DashboardManager _dashboardManager;

    VisualElement _shopItemContainer;

    VisualElement _shopRerollContainer;
    GoldElement _rerollPriceGoldElement;

    VisualElement _shopSellContainer;
    VisualElement _sellItemValueTooltip;

    VisualElement _pouchContainer;

    // Item drag & drop
    bool _buyingItem;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _dashboardManager = GetComponent<DashboardManager>();
        _root = _dashboardManager.Root;
        _dashboardManager.OnShopClicked += OnShopClicked;

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

    void Reroll()
    {
        if (_gameManager.Gold < _gameManager.ShopRerollPrice)
        {
            Helpers.DisplayTextOnElement(_root, _shopRerollContainer, "Insufficient funds", Color.red);
            return;
        }
        int lastPrice = _gameManager.ShopRerollPrice;

        AudioManager.Instance.PlaySFX("DiceRoll", Vector3.zero);
        _gameManager.ChangeGoldValue(-lastPrice);
        _gameManager.ChangeShopRerollPrice(lastPrice *= 2);
        _gameManager.ChooseShopItems();

        DisplayShopItems();
    }

    void DisplayShopItems()
    {
        _shopItemContainer.Clear();
        foreach (var item in _gameManager.ShopItems)
        {
            // so here I want 3 item slots that are filled with items 
            VisualElement container = new();
            container.AddToClassList("shopItem");

            ItemVisual itemVisual = new(item);
            ItemSlotVisual itemSlot = new();
            itemSlot.AddItem(itemVisual);

            //https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events-Handling.html
            itemVisual.RegisterCallback<PointerDownEvent>(OnShopItemPointerDown);

            container.Add(itemSlot);
            container.Add(new GoldElement(item.Price));

            _shopItemContainer.Add(container);
        }
    }

    void OnShopItemPointerDown(PointerDownEvent evt)
    {
        Debug.Log($"on shop intem pointer down");
        if (evt.button != 0)
            return;

        ItemVisual itemVisual = (ItemVisual)evt.target;
        if (itemVisual.Item.Price > base._gameManager.Gold)
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

        // buying
        if (_buyingItem)
            ItemBought();

        base.HandleItemPointerUp();
    }

    void ItemSold()
    {
        _gameManager.ChangeGoldValue(_draggedItem.Item.GetSellValue());

        if (_originalItemSlot.Character != null)
            _originalItemSlot.Character.RemoveItem(_draggedItem.Item);
        else
            _gameManager.RemoveItemFromPouch(_draggedItem.Item);
    }

    void ItemBought()
    {
        _gameManager.ChangeGoldValue(-_draggedItem.Item.Price);
        _gameManager.RemoveItemFromShop(_draggedItem.Item);

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

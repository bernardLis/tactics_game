using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.InputSystem;

public class InventoryUI : Singleton<InventoryUI>
{
    PlayerInput _playerInput;

    InventoryManager _inventoryManager;

    VisualElement _root;
    VisualElement _inventory;
    VisualElement _inventoryContainer;
    ScrollView _inventorySlotContainer;
    VisualElement _inventoryItemInfo;

    public Sprite SlotBackground;
    public Sprite SelectedSlotBackground;
    public InventorySlot SelectedSlot;

    Item _selectedItem;

    public List<InventorySlot> InventorySlots = new List<InventorySlot>();

    protected override void Awake()
    {
        base.Awake();

        _playerInput = GetComponent<PlayerInput>();

        _inventoryManager = GetComponent<InventoryManager>();
        _inventoryManager.OnItemChanged += OnItemChanged;

        // store the root from the ui document component
        _root = GetComponent<UIDocument>().rootVisualElement;

        // search for slot container
        _inventory = _root.Q<VisualElement>("inventory");
        _inventoryContainer = _root.Q<VisualElement>("inventoryContainer");
        _inventorySlotContainer = _root.Q<ScrollView>("inventorySlotContainer");
        _inventoryItemInfo = _root.Q<VisualElement>("inventoryItemInfo");

        _inventorySlotContainer.contentContainer.Clear();
        _inventorySlotContainer.contentContainer.style.flexDirection = FlexDirection.Row;
        _inventorySlotContainer.contentContainer.style.flexWrap = Wrap.Wrap;

        _inventoryItemInfo.Clear();

        // create inventory slots and add them as chlidren to the slot container
        // TODO: resize on amount of items in inventory? 
        for (int i = 0; i < 25; i++)
        {
            InventorySlot item = new InventorySlot();
            InventorySlots.Add(item);
            _inventorySlotContainer.Add(item);
        }

        // populate inventory ui on awake;
        foreach (Item item in _inventoryManager.items)
            AddItemToUI(item);
    }

    void OnEnable()
    {
        _playerInput.actions["InventoryMovement"].performed += ctx => Move(ctx.ReadValue<Vector2>());
        _playerInput.actions["DisableInventoryUI"].performed += ctx => DisableInventoryUI();
        _playerInput.actions["UseItem"].performed += ctx => UseItem();

    }

    void OnDisable()
    {
        if (_playerInput == null)
            return;

        _playerInput.actions["InventoryMovement"].performed -= ctx => Move(ctx.ReadValue<Vector2>());
        _playerInput.actions["DisableInventoryUI"].performed -= ctx => DisableInventoryUI();
        _playerInput.actions["UseItem"].performed -= ctx => UseItem();

    }

    void Move(Vector2 direction)
    {
        // TODO: should not be hardcoded
        float slotContainerWidth = 128f;
        int numberOfItemsInRow = Mathf.FloorToInt(_inventorySlotContainer.resolvedStyle.width / slotContainerWidth);

        if (SelectedSlot == null)
            return;

        // selectedSlot to be overwritten;
        InventorySlot slot = SelectedSlot;

        // https://stackoverflow.com/questions/24799820/get-previous-next-item-of-a-given-item-in-a-list
        // if it is right - select next slot
        if (direction.Equals(Vector2.right))
            slot = InventorySlots.SkipWhile(x => x != SelectedSlot).Skip(1).DefaultIfEmpty(InventorySlots[0]).FirstOrDefault();
        if (direction.Equals(Vector2.left))
            slot = InventorySlots.TakeWhile(x => x != SelectedSlot).DefaultIfEmpty(InventorySlots[InventorySlots.Count - 1]).LastOrDefault();

        // moving up means current item from list - number of items in the row
        if (direction.Equals(Vector2.up))
            slot = InventorySlots[(InventorySlots.Count() + InventorySlots.IndexOf(SelectedSlot) - numberOfItemsInRow) % InventorySlots.Count()];
        // moving down means current item from list + number of items in the row
        if (direction.Equals(Vector2.down))
            slot = InventorySlots[(InventorySlots.IndexOf(SelectedSlot) + numberOfItemsInRow) % InventorySlots.Count()];

        slot.Select();
        _selectedItem = slot.Item;
        _inventorySlotContainer.ScrollTo(slot);
    }

    public void DisplayItemInfo(Item item)
    {
        _inventoryItemInfo.Clear();
        if (item == null)
            return;

        Label name = new Label(item.name);
        name.AddToClassList("inventoryItemInfoTitle");

        Label icon = new Label();
        icon.style.backgroundImage = item.Icon.texture;
        icon.AddToClassList("inventoryItemInfoIcon");

        Label description = new Label(item.Description);
        description.AddToClassList("inventoryItemInfoDescription");

        _inventoryItemInfo.Add(name);
        _inventoryItemInfo.Add(icon);
        _inventoryItemInfo.Add(description);
    }

    public void UnselectSlot()
    {
        if (SelectedSlot != null)
            SelectedSlot.Unselect();
    }

    // helper for selecting slot with mouse
    public void ScrollTo(InventorySlot slot)
    {
        _inventorySlotContainer.ScrollTo(slot);
    }

    void AddItemToUI(Item item)
    {
        // find first empty slot
        var emptySlot = InventorySlots.FirstOrDefault(x => x.Item == null);
        if (emptySlot != null)
            emptySlot.HoldItem(item);
    }

    void RemoveItemFromUI(Item item)
    {
        // TODO: now I am going to remove the first one that I find, does it pose a problem, should I be removing the particular item?
        // find the inventory slot that holds the item
        foreach (var slot in InventorySlots)
        {
            // ask it to drop it
            if (slot.Item == item)
                slot.DropItem();
        }
    }

    // https://www.youtube.com/watch?v=NJLOnRzTPFo&list=PLAE7FECFFFCBE1A54&index=18
    // Inventory.cs sends event
    void OnItemChanged(object sender, ItemChangedEventArgs e)
    {
        if (e.IsAdded)
            AddItemToUI(e.Item);
        else
            RemoveItemFromUI(e.Item);
    }

    public void EnableInventoryUI()
    {
        // switch action map
        _playerInput.SwitchCurrentActionMap("InventoryUI");

        // only one can be visible.
        //GameUI.Instance.HideAllUIPanels();

        _inventoryContainer.style.display = DisplayStyle.Flex;

        // moving around inventory - select top left inventory slot
        InventorySlots.FirstOrDefault().Select();
    }

    public void DisableInventoryUI()
    {
        _inventoryContainer.style.display = DisplayStyle.None;

        // TODO: maybe battle controller can have a method for that; that's nasty
       // CharacterUI.Instance.ShowCharacterUI(BattleCharacterController.Instance.SelectedCharacter.GetComponent<CharacterStats>());

        _playerInput.SwitchCurrentActionMap("Player");
    }

    void UseItem()
    {
        if (_selectedItem.Ability == null)
            return;
        Debug.Log("using item: " + _selectedItem);
        // get current item and queue action
        DisableInventoryUI();

        CharacterUI.Instance.UseItem(_selectedItem);
    }
}

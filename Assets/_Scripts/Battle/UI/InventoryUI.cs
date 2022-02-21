using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();

    VisualElement root;

    VisualElement inventory;
    VisualElement inventoryContainer;
    ScrollView inventorySlotContainer;
    VisualElement inventoryItemInfo;

    public Sprite slotBackground;
    public Sprite selectedSlotBackground;
    public InventorySlot selectedSlot;

    Item selectedItem;

    PlayerInput playerInput;

    InventoryManager inventoryManager;

    public static InventoryUI instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of InventoryUI found");
            return;
        }
        instance = this;
        #endregion

        playerInput = MovePointController.instance.GetComponent<PlayerInput>();

        inventoryManager = InventoryManager.instance;
        inventoryManager.OnItemChanged += OnItemChanged;

        // store the root from the ui document component
        root = GetComponent<UIDocument>().rootVisualElement;

        // search for slot container
        inventory = root.Q<VisualElement>("inventory");
        inventoryContainer = root.Q<VisualElement>("inventoryContainer");
        inventorySlotContainer = root.Q<ScrollView>("inventorySlotContainer");
        inventoryItemInfo = root.Q<VisualElement>("inventoryItemInfo");

        inventorySlotContainer.contentContainer.Clear();
        inventorySlotContainer.contentContainer.style.flexDirection = FlexDirection.Row;
        inventorySlotContainer.contentContainer.style.flexWrap = Wrap.Wrap;

        inventoryItemInfo.Clear();

        // create inventory slots and add them as chlidren to the slot container
        // TODO: resize on amount of items in inventory? 
        for (int i = 0; i < 25; i++)
        {
            InventorySlot item = new InventorySlot();
            inventorySlots.Add(item);
            inventorySlotContainer.Add(item);
        }

        // populate inventory ui on awake;
        foreach (Item item in inventoryManager.items)
        {
            AddItemToUI(item);
        }
    }

    void OnEnable()
    {
        playerInput.actions["InventoryMovement"].performed += ctx => Move(ctx.ReadValue<Vector2>());
        playerInput.actions["DisableInventoryUI"].performed += ctx => DisableInventoryUI();
        playerInput.actions["UseItem"].performed += ctx => UseItem();

    }

    void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["InventoryMovement"].performed -= ctx => Move(ctx.ReadValue<Vector2>());
            playerInput.actions["DisableInventoryUI"].performed -= ctx => DisableInventoryUI();
            playerInput.actions["UseItem"].performed -= ctx => UseItem();
        }
    }

    void Move(Vector2 direction)
    {
        // TODO: should not be hardcoded
        float slotContainerWidth = 128f;
        int numberOfItemsInRow = Mathf.FloorToInt(inventorySlotContainer.resolvedStyle.width / slotContainerWidth);

        if (selectedSlot == null)
            return;

        // selectedSlot to be overwritten;
        InventorySlot slot = selectedSlot;

        // https://stackoverflow.com/questions/24799820/get-previous-next-item-of-a-given-item-in-a-list
        // if it is right - select next slot
        if (direction.Equals(Vector2.right))
            slot = inventorySlots.SkipWhile(x => x != selectedSlot).Skip(1).DefaultIfEmpty(inventorySlots[0]).FirstOrDefault();
        if (direction.Equals(Vector2.left))
            slot = inventorySlots.TakeWhile(x => x != selectedSlot).DefaultIfEmpty(inventorySlots[inventorySlots.Count - 1]).LastOrDefault();

        // moving up means current item from list - number of items in the row
        if (direction.Equals(Vector2.up))
            slot = inventorySlots[(inventorySlots.Count() + inventorySlots.IndexOf(selectedSlot) - numberOfItemsInRow) % inventorySlots.Count()];
        // moving down means current item from list + number of items in the row
        if (direction.Equals(Vector2.down))
            slot = inventorySlots[(inventorySlots.IndexOf(selectedSlot) + numberOfItemsInRow) % inventorySlots.Count()];

        slot.Select();
        selectedItem = slot.item;
        inventorySlotContainer.ScrollTo(slot);
    }

    public void DisplayItemInfo(Item item)
    {
        inventoryItemInfo.Clear();
        if (item == null)
            return;

        Label name = new Label(item.name);
        name.AddToClassList("inventoryItemInfoTitle");

        Label icon = new Label();
        icon.style.backgroundImage = item.icon.texture;
        icon.AddToClassList("inventoryItemInfoIcon");

        Label description = new Label(item.iDescription);
        description.AddToClassList("inventoryItemInfoDescription");

        inventoryItemInfo.Add(name);
        inventoryItemInfo.Add(icon);
        inventoryItemInfo.Add(description);
    }

    public void UnselectSlot()
    {
        if (selectedSlot != null)
            selectedSlot.Unselect();
    }

    // helper for selecting slot with mouse
    public void ScrollTo(InventorySlot slot)
    {
        inventorySlotContainer.ScrollTo(slot);
    }

    void AddItemToUI(Item item)
    {
        // find first empty slot
        var emptySlot = inventorySlots.FirstOrDefault(x => x.item == null);
        if (emptySlot != null)
            emptySlot.HoldItem(item);
    }

    void RemoveItemFromUI(Item item)
    {
        // TODO: now I am going to remove the first one that I find, does it pose a problem, should I be removing the particular item?
        // find the inventory slot that holds the item
        foreach (var slot in inventorySlots)
        {
            // ask it to drop it
            if (slot.item == item)
                slot.DropItem();
        }
    }

    // https://www.youtube.com/watch?v=NJLOnRzTPFo&list=PLAE7FECFFFCBE1A54&index=18
    // Inventory.cs sends event
    void OnItemChanged(object sender, ItemChangedEventArgs e)
    {
        if (e.added)
            AddItemToUI(e.item);
        else
            RemoveItemFromUI(e.item);
    }

    public void EnableInventoryUI()
    {
        // switch action map
        playerInput.SwitchCurrentActionMap("InventoryUI");

        // only one can be visible.
        GameUI.instance.HideAllUIPanels();

        inventoryContainer.style.display = DisplayStyle.Flex;

        // moving around inventory - select top left inventory slot
        inventorySlots.FirstOrDefault().Select();
    }

    public void DisableInventoryUI()
    {
        inventoryContainer.style.display = DisplayStyle.None;

        // TODO: maybe battle controller can have a method for that;
        CharacterUI.instance.ShowCharacterUI(BattleCharacterController.instance.selectedCharacter.GetComponent<CharacterStats>());

        playerInput.SwitchCurrentActionMap("Player");
    }

    void UseItem()
    {
        if (selectedItem.ability == null)
            return;
        Debug.Log("using item: " + selectedItem);
        // get current item and queue action
        DisableInventoryUI();

        CharacterUI.instance.UseItem(selectedItem);
    }
}

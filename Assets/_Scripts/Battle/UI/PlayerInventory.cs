using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class PlayerInventory : Singleton<PlayerInventory>
{
    VisualElement _inventoryGrid;

    static Label _itemDetailHeader;
    static Label _itemDetailBody;
    static Label _itemDetailPrice;

    public static Dimensions SlotDimension { get; private set; }

    public List<StoredItem> StoredItems = new List<StoredItem>();
    public Dimensions InventoryDimensions;

    object _inventoryHasSpace;

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(Configure());
    }

    IEnumerator Configure()
    {
        var root = GetComponentInChildren<UIDocument>().rootVisualElement;
        _inventoryGrid = root.Q<VisualElement>("Grid");

        VisualElement itemDetails = root.Q<VisualElement>("ItemDetails");

        _itemDetailHeader = itemDetails.Q<Label>("Header");
        _itemDetailBody = itemDetails.Q<Label>("Body");
        _itemDetailPrice = itemDetails.Q<Label>("Price");

        //returning 0 will make it wait 1 frame
        yield return 0;

        StartCoroutine(LoadInventory());
    }

    void ConfigureSlotDimensions()
    {
        VisualElement firstSlot = _inventoryGrid.Children().First();

        SlotDimension = new Dimensions
        {
            Width = Mathf.RoundToInt(firstSlot.worldBound.width),
            Height = Mathf.RoundToInt(firstSlot.worldBound.height)
        };
    }

    IEnumerator GetPositionForItem(VisualElement newItem)
    {
        for (int y = 0; y < InventoryDimensions.Height; y++)
        {
            for (int x = 0; x < InventoryDimensions.Width; x++)
            {
                //try position
                SetItemPosition(newItem, new Vector2(SlotDimension.Width * x,
                        SlotDimension.Height * y));

                yield return 0;

                StoredItem overlappingItem = StoredItems.FirstOrDefault(s =>
                        s.RootVisual != null &&
                        s.RootVisual.layout.Overlaps(newItem.layout));
                //Nothing is here! Place the item.
                if (overlappingItem == null)
                {
                    _inventoryHasSpace = true;
                    yield return true;
                }
            }
        }
        _inventoryHasSpace = false;
        yield return false;
    }
    static void SetItemPosition(VisualElement element, Vector2 vector)
    {
        element.style.left = vector.x;
        element.style.top = vector.y;
    }

    IEnumerator LoadInventory()
    {
        foreach (StoredItem loadedItem in StoredItems)
        {
            ItemVisual inventoryItemVisual = new ItemVisual(loadedItem.Details);

            AddItemToInventoryGrid(inventoryItemVisual);
            yield return GetPositionForItem(inventoryItemVisual);
            if ((bool)_inventoryHasSpace == false)
            {
                RemoveItemFromInventoryGrid(inventoryItemVisual);
                yield break;
            }
            ConfigureInventoryItem(loadedItem, inventoryItemVisual);
        }

        yield return null;
    }

    private void AddItemToInventoryGrid(VisualElement item)
    {
        _inventoryGrid.Add(item);
    }
    private void RemoveItemFromInventoryGrid(VisualElement item)
    {
        _inventoryGrid.Remove(item);
    }
    private static void ConfigureInventoryItem(StoredItem item, ItemVisual visual)
    {
        item.RootVisual = visual;
        visual.style.visibility = Visibility.Visible;
    }
}



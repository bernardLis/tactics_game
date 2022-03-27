using UnityEngine;
using UnityEngine.UIElements;

// https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
public class InventorySlot : VisualElement
{
    public Image Icon;
    public Item Item;
    public int Amount;

    public InventorySlot()
    {
        AddToClassList("slotContainer");

        // create a new image element and add it to the root
        Icon = new Image();
        Add(Icon);

        // add uss style properties to the elements
        Icon.AddToClassList("slotIcon");

        RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    public void HoldItem(Item _item)
    {
        Item = _item;
        Icon.sprite = _item.Icon;
    }

    public void DropItem()
    {
        if (Item != null)
            Item = null;
        if (Icon != null && Icon.sprite != null)
            Icon.sprite = null;
    }

    void OnPointerDown(PointerDownEvent evt)
    {
        // || item == null
        if (evt.button != 0)
            return;

        Select();
        InventoryUI.instance.ScrollTo(this);
    }

    public void Select()
    {
        InventoryUI.instance.UnselectSlot();

        //this.style.backgroundColor = Color.gray;
        this.style.backgroundColor = new Color(255 / 255f, 133 / 255f, 125 / 255f, 1); //new Color(0.9f, 0.9f, 0.9f, 1);

        this.style.backgroundImage = new StyleBackground(InventoryUI.instance.SelectedSlotBackground);
        InventoryUI.instance.DisplayItemInfo(Item);
        InventoryUI.instance.SelectedSlot = this;
    }

    public void Unselect()
    {
        this.style.backgroundImage = new StyleBackground(InventoryUI.instance.SlotBackground);
        this.style.backgroundColor = new Color(0.764f, 0.764f, 0.764f, 1);
    }
}

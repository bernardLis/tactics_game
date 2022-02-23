using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
public class InventorySlot : VisualElement
{
    public Image icon;
    public Item item;
    public int amount;

    public InventorySlot()
    {
        AddToClassList("slotContainer");

        // create a new image element and add it to the root
        icon = new Image();
        Add(icon);

        // add uss style properties to the elements
        icon.AddToClassList("slotIcon");

        RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    public void HoldItem(Item _item)
    {
        item = _item;
        icon.sprite = _item.Icon;
    }

    public void DropItem()
    {
        if (item != null)
            item = null;
        if (icon != null && icon.sprite != null)
            icon.sprite = null;
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

        this.style.backgroundImage = new StyleBackground(InventoryUI.instance.selectedSlotBackground);
        InventoryUI.instance.DisplayItemInfo(item);
        InventoryUI.instance.selectedSlot = this;
    }

    public void Unselect()
    {
        this.style.backgroundImage = new StyleBackground(InventoryUI.instance.slotBackground);
        this.style.backgroundColor = new Color(0.764f, 0.764f, 0.764f, 1);
    }


    /* TODO: dragging items in inventory is not a necessary feature for now
	void OnPointerDown(PointerDownEvent evt)
	{
		// TODO: correct? 
		if (evt.button != 0 || item == null)
		{
			return;
		}
		// clear the image
		icon.sprite = null;
		InventoryUI.instance.StartDrag(evt.position, this);
	}
	*/
}

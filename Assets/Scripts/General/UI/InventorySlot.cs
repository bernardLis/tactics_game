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
		// create a new image element and add it to the root
		icon = new Image();
		Add(icon);

		// add uss style properties to the elements
		icon.AddToClassList("slotIcon");
		AddToClassList("slotContainer");

		RegisterCallback<PointerDownEvent>(OnPointerDown);
	}

	public void HoldItem(Item _item)
	{
		item = _item;
		icon.sprite = _item.icon;
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
		// TODO: correct? 
		if (evt.button != 0 || item == null)
		{
			return;
		}
		// clear the image
		icon.sprite = null;
		InventoryUI.instance.StartDrag(evt.position, this);
	}
}

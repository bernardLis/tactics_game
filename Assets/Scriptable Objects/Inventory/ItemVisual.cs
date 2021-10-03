using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


//https://gamedev-resources.com/code-the-grid-based-inventory-system-grid-series-part-2/
public class ItemVisual : VisualElement
{
	private readonly Item item;

	public ItemVisual(Item _item)
	{
		item = _item;
		name = $"{item.iName}";
		style.height = item.slotDimension.height * PlayerInventory.slotDimension.height;
		style.height = item.slotDimension.width * PlayerInventory.slotDimension.width;
		style.visibility = Visibility.Hidden;
		VisualElement icon = new VisualElement
		{
			style = { backgroundImage = item.icon.texture }
		};
		Add(icon);
		icon.AddToClassList("visual-icon");
		AddToClassList("visual-icon-container");
	}

	public void SetPosition(Vector2 pos)
	{
		style.left = pos.x;
		style.top = pos.y;
	}
}

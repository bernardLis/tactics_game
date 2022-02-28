using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


//https://gamedev-resources.com/code-the-grid-based-inventory-system-grid-series-part-2/
public class ItemVisual : VisualElement
{
	private readonly Item item;

	public ItemVisual(Item item)
	{
		this.item = item;
        name = $"{this.item.name}";
        style.height = this.item.SlotDimension.Height * PlayerInventory.SlotDimension.Height;
        style.height = this.item.SlotDimension.Width * PlayerInventory.SlotDimension.Width;
		style.visibility = Visibility.Hidden;
        VisualElement icon = new VisualElement
        {
			style = { backgroundImage = this.item.Icon.texture }
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

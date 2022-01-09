using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : BaseScriptableObject
{
	public string ID = Guid.NewGuid().ToString();
	public string iName = "New Item";
	public string iDescription;
	public Sprite icon = null;
	public int amount = 1;
	public int sellPrice;
	public int stackSize = 5;
	public bool questItem;
	public Dimensions slotDimension;

	public virtual void PickUp()
	{
		// TODO: maybe a bad idea? 
		Inventory.instance.Add(this);
	}

	public virtual void Use()
	{
		// meant to be overwritten
	}


}

[Serializable]
public struct Dimensions
{
	public int height;
	public int width;
}

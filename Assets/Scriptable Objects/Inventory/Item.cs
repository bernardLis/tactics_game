using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
	public string iName = "New Item";
	public Sprite icon = null;
	public int iAmount = 1;
	public int stackSize = 5;

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

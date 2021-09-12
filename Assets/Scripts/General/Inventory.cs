using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// https://www.youtube.com/watch?v=NJLOnRzTPFo&list=PLAE7FECFFFCBE1A54&index=18
public class ItemChangedEventArgs : EventArgs
{
	public Item item { get; private set; }
	public bool added;
	public ItemChangedEventArgs(Item _item, bool _added)
	{
		item = _item;
		added = _added;
	}
}

public class Inventory : MonoBehaviour
{
	public List<Item> items = new List<Item>();

	public event EventHandler<ItemChangedEventArgs> OnItemChanged;

	/*
	public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;
*/

	#region Singleton
	public static Inventory instance;
	void Awake()
	{
		// singleton
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of Inventory found");
			return;
		}
		instance = this;
	}
	#endregion

	public void Add(Item item)
	{
		items.Add(item);

		GameUI.instance.DisplayLogText("+ 1 " + item.iName);
		if (OnItemChanged != null)
			OnItemChanged(this, new ItemChangedEventArgs(item, true));

		/*
		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
			*/
	}

	public void Remove(Item item)
	{
		items.Remove(item);

		if (OnItemChanged != null)
			OnItemChanged(this, new ItemChangedEventArgs(item, false));
	}
}

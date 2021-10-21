using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;

public class PlayerInventory : MonoBehaviour
{
	VisualElement root;
	VisualElement inventoryGrid;

	static Label itemDetailHeader;
	static Label itemDetailBody;
	static Label itemDetailPrice;

	public static Dimensions slotDimension { get; private set; }

	public List<StoredItem> storedItems = new List<StoredItem>();
	public Dimensions inventoryDimensions;

	object inventoryHasSpace;
	public static PlayerInventory Instance;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			StartCoroutine(Configure());
		}
		else if (Instance != this)
		{
			Destroy(this);
		}
	}

	IEnumerator Configure()
	{
		root = GetComponentInChildren<UIDocument>().rootVisualElement;
		inventoryGrid = root.Q<VisualElement>("Grid");

		VisualElement itemDetails = root.Q<VisualElement>("ItemDetails");

		itemDetailHeader = itemDetails.Q<Label>("Header");
		itemDetailBody = itemDetails.Q<Label>("Body");
		itemDetailPrice = itemDetails.Q<Label>("Price");

		//returning 0 will make it wait 1 frame
		yield return 0;

		StartCoroutine(LoadInventory());
	}

	void ConfigureSlotDimensions()
	{
		VisualElement firstSlot = inventoryGrid.Children().First();

		slotDimension = new Dimensions
		{
			width = Mathf.RoundToInt(firstSlot.worldBound.width),
			height = Mathf.RoundToInt(firstSlot.worldBound.height)
		};
	}

	IEnumerator GetPositionForItem(VisualElement newItem)
	{
		for (int y = 0; y < inventoryDimensions.height; y++)
		{
			for (int x = 0; x < inventoryDimensions.width; x++)
			{
				//try position
				SetItemPosition(newItem, new Vector2(slotDimension.width * x,
						slotDimension.height * y));

				yield return 0;

				StoredItem overlappingItem = storedItems.FirstOrDefault(s =>
						s.rootVisual != null &&
						s.rootVisual.layout.Overlaps(newItem.layout));
				//Nothing is here! Place the item.
				if (overlappingItem == null)
				{
					inventoryHasSpace = true;
					yield return true;
				}
			}
		}
		inventoryHasSpace = false;
		yield return false;
	}
	static void SetItemPosition(VisualElement element, Vector2 vector)
	{
		element.style.left = vector.x;
		element.style.top = vector.y;
	}

	IEnumerator LoadInventory()
	{
		foreach (StoredItem loadedItem in storedItems)
		{
			ItemVisual inventoryItemVisual = new ItemVisual(loadedItem.details);

			AddItemToInventoryGrid(inventoryItemVisual);
			yield return GetPositionForItem(inventoryItemVisual);
			if ((bool)inventoryHasSpace == false)
			{
				Debug.Log("No space - Cannot pick up the item");
				RemoveItemFromInventoryGrid(inventoryItemVisual);
				yield break;
			}
			ConfigureInventoryItem(loadedItem, inventoryItemVisual);
		}

		yield return null;
	}

	private void AddItemToInventoryGrid(VisualElement item)
	{
		inventoryGrid.Add(item);
	}
	private void RemoveItemFromInventoryGrid(VisualElement item)
	{
		inventoryGrid.Remove(item);
	}
	private static void ConfigureInventoryItem(StoredItem item, ItemVisual visual)
	{
		item.rootVisual = visual;
		visual.style.visibility = Visibility.Visible;
	}
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class InventoryUIController : MonoBehaviour
{

	public List<InventorySlot> inventoryItems = new List<InventorySlot>();
	VisualElement root;
	VisualElement slotContainer;

	VisualElement ghostIcon;
	bool isDragging;
	InventorySlot originalSlot;

	public static InventoryUIController instance;

	void Awake()
	{
		// singleton
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of InventoryUIController found");
			return;
		}
		instance = this;

		Inventory.instance.onItemChangedCallback += OnItemChanged;
		// store the root from the ui document component
		root = GetComponent<UIDocument>().rootVisualElement;

		// search for slot container
		slotContainer = root.Q<VisualElement>("inventorySlotContainer");
		ghostIcon = root.Q<VisualElement>("inventoryGhostIcon");

		ghostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
		ghostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);

		// create inventory slotsd and add them as chlidren to the slot container
		for (int i = 0; i < 20; i++)
		{
			InventorySlot item = new InventorySlot();
			inventoryItems.Add(item);
			slotContainer.Add(item);
		}

		// populate inventory ui on awake;
		foreach (Item item in Inventory.instance.items)
		{
			// find first empty slot
			var emptySlot = inventoryItems.FirstOrDefault(x => x.item == null);
			if (emptySlot != null)
			{
				emptySlot.HoldItem(item);
			}
		}
	}

	void OnItemChanged()
	{
		print("on item chagne in inventory ui controller");
	}


	// TODO: should I disable it when inventory window is closed?
	// https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
	public void StartDrag(Vector2 position, InventorySlot _originalSlot)
	{
		isDragging = true;
		originalSlot = _originalSlot;

		ghostIcon.style.top = position.y - ghostIcon.layout.height / 2;
		ghostIcon.style.left = position.x - ghostIcon.layout.width / 2;

		ghostIcon.style.backgroundImage = originalSlot.item.icon.texture;

		ghostIcon.style.visibility = Visibility.Visible;
	}

	void OnPointerMove(PointerMoveEvent evt)
	{
		// Only take action if the player is dragging an item around the screen
		if (!isDragging)
		{
			return;
		}

		ghostIcon.style.top = evt.position.y - ghostIcon.layout.height / 2;
		ghostIcon.style.left = evt.position.x - ghostIcon.layout.width / 2;
	}

	void OnPointerUp(PointerUpEvent evt)
	{
		if (!isDragging)
		{
			return;
		}

		// Check to see if they are dropping the ghost icon over any inventory slots.
		IEnumerable<InventorySlot> slots = inventoryItems.Where(x => x.worldBound.Overlaps(ghostIcon.worldBound));
		if (slots.Count() != 0)
		{
			InventorySlot closestSlot = slots.OrderBy(x => Vector2.Distance(x.worldBound.position, ghostIcon.worldBound.position)).First();
			closestSlot.HoldItem(originalSlot.item);
			originalSlot.DropItem();
		}
		// Didn't find any (dragged off the window)
		else
		{
			originalSlot.icon.image = originalSlot.item.icon.texture;
		}

		isDragging = false;
		originalSlot = null;
		ghostIcon.style.visibility = Visibility.Hidden;


	}
}

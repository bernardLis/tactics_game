using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{

	public List<InventorySlot> inventorySlots = new List<InventorySlot>();

	VisualElement root;
	VisualElement inventoryContainer;
	VisualElement slotContainer;

	VisualElement ghostIcon;
	bool isDragging;
	InventorySlot originalSlot;

	public InputMaster controls;

	GameObject player;


	public static InventoryUI instance;


	void Awake()
	{
		// singleton
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of InventoryUI found");
			return;
		}
		instance = this;

		controls = new InputMaster();
		controls.FMPlayer.EnableInventoryUI.performed += ctx => EnableInventoryUI();

		controls.InventoryUI.Test.performed += ctx => Test();
		controls.InventoryUI.DisableInventoryUI.performed += ctx => DisableInventoryUI();

		player = GameObject.FindGameObjectWithTag("Player");

		Inventory.instance.OnItemChanged += OnItemChanged;
		// store the root from the ui document component
		root = GetComponent<UIDocument>().rootVisualElement;

		// search for slot container
		inventoryContainer = root.Q<VisualElement>("inventoryContainer");
		slotContainer = root.Q<VisualElement>("inventorySlotContainer");
		ghostIcon = root.Q<VisualElement>("inventoryGhostIcon");

		ghostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
		ghostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);

		// create inventory slotsd and add them as chlidren to the slot container
		for (int i = 0; i < 20; i++)
		{
			InventorySlot item = new InventorySlot();
			inventorySlots.Add(item);
			slotContainer.Add(item);
		}

		// populate inventory ui on awake;
		foreach (Item item in Inventory.instance.items)
		{
			AddItemToUI(item);
		}
	}

	void AddItemToUI(Item item)
	{
		/*
		// check if you already have this item, if yes, stack it
		foreach (var slot in inventorySlots)
		{
			if (slot.item == item)
				print("i already have this item, i would like to stack it");
		}
		*/

		// find first empty slot
		var emptySlot = inventorySlots.FirstOrDefault(x => x.item == null);
		if (emptySlot != null)
		{
			emptySlot.HoldItem(item);
		}
	}

	void RemoveItemFromUI(Item item)
	{
		// TODO: now I am going to remove the first one that I find, does it pose a problem, should I be removing the particular item?
		// find the inventory slot that holds the item
		foreach (var slot in inventorySlots)
		{
			// ask it to drop it
			if (slot.item == item)
				slot.DropItem();
		}
	}


	void OnEnable()
	{
		controls.FMPlayer.Enable();
	}

	void OnDisable()
	{
		controls.FMPlayer.Disable();
	}

	// https://www.youtube.com/watch?v=NJLOnRzTPFo&list=PLAE7FECFFFCBE1A54&index=18
	// Inventory.cs sends event
	void OnItemChanged(object sender, ItemChangedEventArgs e)
	{
		if (e.added)
			AddItemToUI(e.item);
		else
			RemoveItemFromUI(e.item);
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
		IEnumerable<InventorySlot> slots = inventorySlots.Where(x => x.worldBound.Overlaps(ghostIcon.worldBound));
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

	// Inputs
	void Test()
	{
		print("test");
	}

	void EnableInventoryUI()
	{
		inventoryContainer.style.display = DisplayStyle.Flex;
		// TODO: only controls.FMPlayer.Disable() does not disable player controlls
		controls.FMPlayer.Disable();
		player.SetActive(false);

		GameManager.instance.PauseGame();

		controls.InventoryUI.Enable();
	}

	void DisableInventoryUI()
	{
		inventoryContainer.style.display = DisplayStyle.None;

		controls.FMPlayer.Enable();
		player.SetActive(true);
		GameManager.instance.ResumeGame();

		controls.InventoryUI.Disable();
	}


}

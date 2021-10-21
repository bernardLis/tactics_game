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

	VisualElement inventory;
	VisualElement inventoryContainer;
	ScrollView inventorySlotContainer;
	VisualElement inventoryItemInfo;

	public Sprite slotBackground;
	public Sprite selectedSlotBackground;
	public InventorySlot selectedSlot;

	GameObject player;
	PlayerInput playerInput;

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

		playerInput = MovePointController.instance.GetComponent<PlayerInput>();

		Inventory.instance.OnItemChanged += OnItemChanged;
		// store the root from the ui document component
		root = GetComponent<UIDocument>().rootVisualElement;

		// search for slot container
		inventory = root.Q<VisualElement>("inventory");
		inventoryContainer = root.Q<VisualElement>("inventoryContainer");
		inventorySlotContainer = root.Q<ScrollView>("inventorySlotContainer");
		inventoryItemInfo = root.Q<VisualElement>("inventoryItemInfo");

		inventorySlotContainer.contentContainer.Clear();
		inventorySlotContainer.contentContainer.style.flexDirection = FlexDirection.Row;
		inventorySlotContainer.contentContainer.style.flexWrap = Wrap.Wrap;

		inventoryItemInfo.Clear();

		// create inventory slots and add them as chlidren to the slot container
		// TODO: resize on amount of items in inventory? 
		for (int i = 0; i < 25; i++)
		{
			InventorySlot item = new InventorySlot();
			inventorySlots.Add(item);
			inventorySlotContainer.Add(item);
		}

		// populate inventory ui on awake;
		foreach (Item item in Inventory.instance.items)
		{
			AddItemToUI(item);
		}
	}

	void OnEnable()
	{
		playerInput.actions["InventoryMovement"].performed += ctx => Move(ctx.ReadValue<Vector2>());
		playerInput.actions["DisableInventoryUI"].performed += ctx => DisableInventoryUI();
	}

	void OnDisable()
	{
		if (playerInput != null)
		{
			playerInput.actions["InventoryMovement"].performed += ctx => Move(ctx.ReadValue<Vector2>());
			playerInput.actions["DisableInventoryUI"].performed -= ctx => DisableInventoryUI();
		}
	}

	void Move(Vector2 direction)
	{
		// TODO: should not be hardcoded
		float slotContainerWidth = 128f;
		int numberOfItemsInRow = Mathf.FloorToInt(inventorySlotContainer.resolvedStyle.width / slotContainerWidth);

		if (selectedSlot == null)
			return;

		// selectedSlot to be overwritten;
		InventorySlot slot = selectedSlot;

		// https://stackoverflow.com/questions/24799820/get-previous-next-item-of-a-given-item-in-a-list
		// if it is right - select next slot
		if (direction.Equals(Vector2.right))
			slot = inventorySlots.SkipWhile(x => x != selectedSlot).Skip(1).DefaultIfEmpty(inventorySlots[0]).FirstOrDefault();
		if (direction.Equals(Vector2.left))
			slot = inventorySlots.TakeWhile(x => x != selectedSlot).DefaultIfEmpty(inventorySlots[inventorySlots.Count - 1]).LastOrDefault();

		// moving up means current item from list - number of items in the row
		if (direction.Equals(Vector2.up))
			slot = inventorySlots[(inventorySlots.Count() + inventorySlots.IndexOf(selectedSlot) - numberOfItemsInRow) % inventorySlots.Count()];
		// moving down means current item from list + number of items in the row
		if (direction.Equals(Vector2.down))
			slot = inventorySlots[(inventorySlots.IndexOf(selectedSlot) + numberOfItemsInRow) % inventorySlots.Count()];

		slot.Select();
		inventorySlotContainer.ScrollTo(slot);
	}

	public void DisplayItemInfo(Item item)
	{
		inventoryItemInfo.Clear();
		if (item == null)
			return;

		Label name = new Label(item.iName);
		name.AddToClassList("inventoryItemInfoTitle");

		Label icon = new Label();
		icon.style.backgroundImage = item.icon.texture;
		icon.AddToClassList("inventoryItemInfoIcon");

		Label description = new Label(item.iDescription);
		description.AddToClassList("inventoryItemInfoDescription");

		inventoryItemInfo.Add(name);
		inventoryItemInfo.Add(icon);
		inventoryItemInfo.Add(description);
	}

	public void UnselectSlot()
	{
		if (selectedSlot != null)
			selectedSlot.Unselect();
	}

	// helper for selecting slot with mouse
	public void ScrollTo(InventorySlot slot)
	{
		inventorySlotContainer.ScrollTo(slot);
	}

	void AddItemToUI(Item item)
	{
		// find first empty slot
		var emptySlot = inventorySlots.FirstOrDefault(x => x.item == null);
		if (emptySlot != null)
			emptySlot.HoldItem(item);
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

	// https://www.youtube.com/watch?v=NJLOnRzTPFo&list=PLAE7FECFFFCBE1A54&index=18
	// Inventory.cs sends event
	void OnItemChanged(object sender, ItemChangedEventArgs e)
	{
		if (e.added)
			AddItemToUI(e.item);
		else
			RemoveItemFromUI(e.item);
	}

	public void EnableInventoryUI()
	{
		// switch action map
		player.GetComponent<PlayerInput>().SwitchCurrentActionMap("InventoryUI");
		GameManager.instance.PauseGame();

		// only one can be visible.
		GameUI.instance.HideAllUIPanels();

		inventoryContainer.style.display = DisplayStyle.Flex;

		// moving around inventory - select top left inventory slot
		inventorySlots.FirstOrDefault().Select();
	}

	public void DisableInventoryUI()
	{
		inventoryContainer.style.display = DisplayStyle.None;

		GameManager.instance.EnableFMPlayerControls();
		GameManager.instance.ResumeGame();
	}

	/* TODO: dragging is not a necessary feature for now

	in awake: 
		//VisualElement ghostIcon;
	bool isDragging;

			//ghostIcon = root.Q<VisualElement>("inventoryGhostIcon");
				InventorySlot originalSlot;


			//ghostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
		//ghostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);

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
	*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class BlacksmithManager : MonoBehaviour
{
    GameManager _gameManager;

    VisualElement _root;

    VisualElement _blacksmithItemContainer;

    VisualElement _blacksmithSellContainer;

    VisualElement _playerItemPouch;
    Label _blacksmithPlayerObolsAmount;

    bool _wasVisited;


    // drag & drop
    // https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
    bool _isDragging;
    ItemSlotVisual _originalSlot;
    VisualElement _dragDropContainer;
    ItemVisual _draggedItem;

    List<ItemSlotVisual> _playerItemSlotVisuals = new();

    void Awake()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnObolsChanged += OnObolsChanged;


        _root = GetComponent<UIDocument>().rootVisualElement;

        _blacksmithItemContainer = _root.Q<VisualElement>("blacksmithItemContainer");
        _blacksmithSellContainer = _root.Q<VisualElement>("blacksmithSellContainer");

        Button blacksmithRerollButton = _root.Q<Button>("blacksmithRerollButton");
        blacksmithRerollButton.clickable.clicked += Reroll;

        _playerItemPouch = _root.Q<VisualElement>("playerItemPouch");
        _blacksmithPlayerObolsAmount = _root.Q<Label>("blacksmithPlayerObolsAmount");

        Button blacksmithBackButton = _root.Q<Button>("blacksmithBackButton");
        blacksmithBackButton.clickable.clicked += Back;

        Initialize();

        //drag and drop
        _dragDropContainer = _root.Q<VisualElement>("dragDropContainer");

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnDestroy()
    {
        _gameManager.OnObolsChanged -= OnObolsChanged;
    }

    void OnObolsChanged(int amount)
    {
        _blacksmithPlayerObolsAmount.text = "" + amount;
    }

    public void Initialize()
    {
        if (!_wasVisited)
            PopulateItems();

        _wasVisited = true;
        PopulatePlayerPouch();
    }

    void Reroll()
    {
        _gameManager.SetObols(_gameManager.Obols - 1);
        PopulateItems();
    }

    void PopulateItems()
    {
        _blacksmithItemContainer.Clear();
        for (int i = 0; i < 3; i++)
        {
            // so here I want 3 item slots that are filled with items 
            VisualElement container = new();

            Item item = _gameManager.CharacterDatabase.GetRandomItem();
            ItemVisual iv = new(item);
            ItemSlotVisual itemSlot = new(iv);

            //https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events-Handling.html
            iv.RegisterCallback<PointerDownEvent>(OnItemVisualPointerDown);

            container.Add(itemSlot);
            container.Add(new Label("Price: " + item.Price)); // TODO: coin logo instead of word "price" 

            _blacksmithItemContainer.Add(container);
        }
    }

    void PopulatePlayerPouch()
    {
        _playerItemPouch.Clear();
        _playerItemSlotVisuals.Clear();

        for (int i = 0; i < 3; i++)
        {
            ItemSlotVisual slot = new ItemSlotVisual();
            _playerItemPouch.Add(slot);
            _playerItemSlotVisuals.Add(slot);
        }

        for (int i = 0; i < _gameManager.PlayerItemPouch.Count; i++)
        {
            ItemVisual itemVisual = new(_gameManager.PlayerItemPouch[i]);
            _playerItemSlotVisuals[i].Add(itemVisual);
        }

    }

    void Back()
    {
        ShopManager.Instance.ShowShops();
        _gameManager.SaveJsonData();
    }

    void OnItemVisualPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        ItemVisual itemVisual = (ItemVisual)evt.target;
        ItemSlotVisual itemSlotVisual = (ItemSlotVisual)itemVisual.parent;

        itemSlotVisual.Remove(itemVisual);

        StartDrag(evt.position, itemSlotVisual, itemVisual);
    }

    //drag & drop
    public void StartDrag(Vector2 position, ItemSlotVisual originalSlot, ItemVisual draggedItem)
    {
        _draggedItem = draggedItem;

        //Set tracking variables
        _isDragging = true;
        _originalSlot = originalSlot;
        //Set the new position
        _dragDropContainer.style.top = position.y - _dragDropContainer.layout.height / 2;
        _dragDropContainer.style.left = position.x - _dragDropContainer.layout.width / 2;
        //Set the image
        _dragDropContainer.Add(draggedItem);
        //Flip the visibility on
        _dragDropContainer.style.visibility = Visibility.Visible;
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!_isDragging)
            return;

        //Set the new position
        _dragDropContainer.style.top = evt.position.y - _dragDropContainer.layout.height / 2;
        _dragDropContainer.style.left = evt.position.x - _dragDropContainer.layout.width / 2;
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging)
            return;

        if (ItemSold())
            return;



        //Check to see if they are dropping the ghost icon over any inventory slots.
        IEnumerable<ItemSlotVisual> slots = _playerItemSlotVisuals.Where(x =>
               x.worldBound.Overlaps(_dragDropContainer.worldBound));

        //Found at least one
        if (slots.Count() != 0)
        {
            ItemSlotVisual closestSlot = slots.OrderBy(x => Vector2.Distance
               (x.worldBound.position, _dragDropContainer.worldBound.position)).First();

            //Set the new inventory slot with the data
            closestSlot.AddItem(_draggedItem);
        }
        //Didn't find any (dragged off the window)
        else
        {
            _originalSlot.AddItem(_draggedItem);
        }

        DragCleanUp();
    }

    bool ItemSold()
    {
        // TODO: check if you owned the item
        if (!_blacksmithSellContainer.worldBound.Overlaps(_dragDropContainer.worldBound))
            return false;

        // add money // destroy item
        _gameManager.PlayerItemPouch.Remove(_draggedItem.Item);
        _gameManager.SetObols(_gameManager.Obols + Mathf.FloorToInt(_draggedItem.Item.Price * 0.5f));
        // TODO: damn... I need to differentiate between selling from the pouch and the  

        DragCleanUp();
        return true;
    }

    void DragCleanUp()
    {
        //Clear dragging related visuals and data
        _isDragging = false;
        _originalSlot = null;
        _draggedItem = null;
        _dragDropContainer.Clear();
        _dragDropContainer.style.visibility = Visibility.Hidden;
    }


}

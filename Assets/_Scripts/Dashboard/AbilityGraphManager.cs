using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityGraphManager : MonoBehaviour
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;

    List<AbilityNodeGraph> _abilityNodeGraphs;


    VisualElement _root;
    VisualElement _abilityGraphs;
    VisualElement _craftAbilityNodeSlotContainer;
    AbilityNodeSlotVisualElement _craftAbilityNodeSlot;


    // drag & drop
    // https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
    bool _isDragging;

    VisualElement _dragDropContainer;
    AbilityNodeVisualElement _draggedNode;

    public event Action<AbilityNodeVisualElement> OnCraftNodeAdded;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _abilityNodeGraphs = _gameManager.GetAbilityNodeGraphs();

        _dashboardManager = GetComponent<DashboardManager>();
        _dashboardManager.OnAbilitiesOpened += OnAbilitiesClicked;
        _root = _dashboardManager.Root;

        _abilityGraphs = _root.Q<VisualElement>("abilityGraphs");
        _abilityGraphs.Clear();

        CreateGraphs();

        _craftAbilityNodeSlotContainer = _root.Q<VisualElement>("craftAbilityNodeSlotContainer");
        _craftAbilityNodeSlot = new();
        _craftAbilityNodeSlotContainer.Add(_craftAbilityNodeSlot);
        _craftAbilityNodeSlot.OnNodeAdded += CraftNodeAdded; // HERE:

        // drag & drop
        _dragDropContainer = new VisualElement();
        _dragDropContainer.AddToClassList("abilityNodeDragAndDrop");
        _root.Add(_dragDropContainer);

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void CraftNodeAdded(AbilityNodeVisualElement nodeVisualElement) { OnCraftNodeAdded?.Invoke(nodeVisualElement); }

    void OnAbilitiesClicked()
    {

    }

    public void ClearCraftSlot()
    {
        _craftAbilityNodeSlot.RemoveNode();
    }

    void CreateGraphs()
    {
        foreach (AbilityNodeGraph graph in _abilityNodeGraphs)
        {
            VisualElement graphContainer = new();
            graphContainer.AddToClassList("graphContainer");
            graphContainer.AddToClassList("textPrimary");
            _abilityGraphs.Add(graphContainer);

            Label title = new(graph.Title);
            title.style.fontSize = 48;
            graphContainer.Add(title);

            VisualElement wrapper = new();
            wrapper.style.flexDirection = FlexDirection.Row;
            graphContainer.Add(wrapper);

            for (int i = 0; i < graph.AbilityNodes.Length; i++)
                wrapper.Add(CreateNodeElement(graph.AbilityNodes[i]));
        }
    }

    VisualElement CreateNodeElement(AbilityNode node)
    {
        VisualElement container = new();
        AbilityNodeVisualElement el = new(node);
        el.RegisterCallback<PointerDownEvent>(OnNodePointerDown);

        AbilityNodeSlotVisualElement slot = new(el, node.IsUnlocked);

        container.Add(slot);
        if (!node.IsUnlocked)
        {
            VisualElement spiceElementContainer = GetNodeCostElement(node);
            container.Add(spiceElementContainer);
            el.AddCostElement(spiceElementContainer.Q<SpiceElement>());
        }

        return container;

    }

    void OnNodePointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        AbilityNodeVisualElement node = (AbilityNodeVisualElement)evt.currentTarget;
        if (!node.AbilityNode.IsUnlocked)
            return;

        AbilityNodeVisualElement nodeCopy = new(node.AbilityNode);
        nodeCopy.BlockTooltip();
        StartNodeDrag(evt.position, nodeCopy);
    }

    void StartNodeDrag(Vector2 position, AbilityNodeVisualElement draggedNode)
    {
        _draggedNode = draggedNode;

        //Set tracking variables
        _isDragging = true;

        //Set the new position
        _dragDropContainer.BringToFront();
        _dragDropContainer.style.top = position.y - _dragDropContainer.layout.height / 2;
        _dragDropContainer.style.left = position.x - _dragDropContainer.layout.width / 2;
        //Set the image
        _dragDropContainer.Add(draggedNode);
        //Flip the visibility on
        _dragDropContainer.style.visibility = Visibility.Visible;
    }

    void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!_isDragging)
            return;

        if (_draggedNode != null)
        {
            //Set the new position
            _dragDropContainer.style.top = evt.position.y - _dragDropContainer.layout.height / 2;
            _dragDropContainer.style.left = evt.position.x - _dragDropContainer.layout.width / 2;
        }
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging)
            return;

        if (_draggedNode != null)
            HandleNodePointerUp();
    }

    void HandleNodePointerUp()
    {
        if (_dragDropContainer.worldBound.Overlaps(_craftAbilityNodeSlot.worldBound))
            _craftAbilityNodeSlot.AddNode(_draggedNode);

        _dragDropContainer.Clear();
        _draggedNode = null;
        _isDragging = false;
        _dragDropContainer.style.visibility = Visibility.Hidden;
    }

    VisualElement GetNodeCostElement(AbilityNode abilityNode)
    {
        VisualElement cost = new();
        cost.style.flexDirection = FlexDirection.Row;
        cost.style.justifyContent = Justify.Center;
        cost.Add(new SpiceElement(abilityNode.SpiceCost));
        return cost;
    }
}



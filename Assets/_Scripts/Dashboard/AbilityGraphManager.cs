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
    public AbilityNodeSlot CraftAbilityNodeSlot { get; private set; }

    // drag & drop
    // https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
    bool _isDragging;

    VisualElement _dragDropContainer;
    AbilityNodeElement _draggedNode;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "ability-crafting__";
    const string _ussGraphContainer = _ussClassName + "graph-container";
    const string _ussNodeDragAndDrop = _ussClassName + "node-drag-and-drop";
    const string _ussNodeCostElement = _ussClassName + "node-cost-element";

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

        VisualElement craftAbilityNodeSlotContainer = _root.Q<VisualElement>("craftAbilityNodeSlotContainer");
        CraftAbilityNodeSlot = new();
        craftAbilityNodeSlotContainer.Add(CraftAbilityNodeSlot);
        CraftAbilityNodeSlot.OnNodeAdded += CraftNodeAdded;

        // drag & drop
        _dragDropContainer = new VisualElement();
        _dragDropContainer.AddToClassList(_ussNodeDragAndDrop);
        _root.Add(_dragDropContainer);

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void CraftNodeAdded(AbilityNodeElement nodeVisualElement) { nodeVisualElement.RegisterCallback<PointerUpEvent>(RightClickToRemove); }

    void OnAbilitiesClicked()
    {
        // TODO: should I reload?
    }

    void RightClickToRemove(PointerUpEvent evt)
    {
        if (evt.button != 1)
            return;

        ClearCraftSlot();
    }

    public void ClearCraftSlot() { CraftAbilityNodeSlot.RemoveNode(); }

    void CreateGraphs()
    {
        foreach (AbilityNodeGraph graph in _abilityNodeGraphs)
        {
            VisualElement graphContainer = new();
            graphContainer.AddToClassList(_ussGraphContainer);
            graphContainer.AddToClassList(_ussCommonTextPrimary);
            _abilityGraphs.Add(graphContainer);

            Label title = new(graph.Title);
            title.style.fontSize = 48;
            graphContainer.Add(title);

            VisualElement wrapper = new();
            wrapper.style.flexDirection = FlexDirection.Row;
            graphContainer.Add(wrapper);

            for (int i = 0; i < graph.AbilityNodes.Length; i++)
            {
                graph.AbilityNodes[i].Initialize();
                wrapper.Add(CreateNodeElement(graph.AbilityNodes[i]));
            }
        }
    }

    VisualElement CreateNodeElement(AbilityNode node)
    {
        VisualElement container = new();
        AbilityNodeElement el = new(node);
        el.RegisterCallback<PointerDownEvent>(OnNodePointerDown);

        AbilityNodeSlot slot = new(el, node.IsUnlocked);

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

        AbilityNodeElement node = (AbilityNodeElement)evt.currentTarget;
        if (!node.AbilityNode.IsUnlocked)
            return;
        if (node.AbilityNode.IsOnCooldown)
        {
            node.ShakeNode();
            return;
        }

        AbilityNodeElement nodeCopy = new(node.AbilityNode);
        nodeCopy.BlockTooltip();
        StartNodeDrag(evt.position, nodeCopy);
    }

    void StartNodeDrag(Vector2 position, AbilityNodeElement draggedNode)
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
        if (_dragDropContainer.worldBound.Overlaps(CraftAbilityNodeSlot.worldBound))
            CraftAbilityNodeSlot.AddNode(_draggedNode);

        _dragDropContainer.Clear();
        _draggedNode = null;
        _isDragging = false;
        _dragDropContainer.style.visibility = Visibility.Hidden;
    }

    VisualElement GetNodeCostElement(AbilityNode abilityNode)
    {
        VisualElement cost = new();
        cost.AddToClassList(_ussNodeCostElement);
        cost.Add(new SpiceElement(abilityNode.SpiceCost));
        return cost;
    }

}


